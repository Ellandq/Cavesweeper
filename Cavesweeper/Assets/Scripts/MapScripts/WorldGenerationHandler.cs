using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class WorldGenerationHandler : MonoBehaviour
{
    public static WorldGenerationHandler Instance;

    [Header ("World Information")]
    private int trappedRoomsCount;
    [SerializeField] private Vector2Int startingPosition;
    [SerializeField] private Vector2Int endPosition;
    private float roomDistance = 7.5f;

    [Header ("Room Information")]
    [SerializeField] private short[] roomTypeCounts;
    private byte[,] dangerLevelArray;

    [Header ("Room Prefabs")]
    [SerializeField] private GameObject defaultRoomPrefab;
    [SerializeField] private List<GameObject> emptyRoomPrefabs;
    [SerializeField] private List<GameObject> spikeTrapRoomPrefabs;
    [SerializeField] private List<GameObject> gasTrapRoomPrefabs;
    [SerializeField] private List<GameObject> monsterTrapRoomPrefab;
    [SerializeField] private List<GameObject> treasureRoomPrefabs;

    [Header ("Object References")]
    [SerializeField] private Transform roomManager;

    private void Awake (){
        Instance = this;
    }

    public void GenerateWorld (int trappedRoomsCount){
        this.trappedRoomsCount = trappedRoomsCount;

        // Initializing the array
        dangerLevelArray = new byte[GameManager.worldSize.x, GameManager.worldSize.y];
        roomTypeCounts = new short[5];

        Random.InitState(GameManager.worldSeed);

        GenerateStartAndEndPoint();
        SetUpRoomTypeCounts();
        RoomType[,] roomTypeArray = GenerateRooms();

        GenerateRoomStructures(roomTypeArray);
    }

    private void GenerateStartAndEndPoint (){
        int y = Random.Range(0, GameManager.worldSize.y);
        startingPosition = new Vector2Int(0, y);

        y = Random.Range(0, GameManager.worldSize.y);
        endPosition = new Vector2Int(GameManager.worldSize.x - 1, y);
    }

    private void SetUpRoomTypeCounts (){
        short totalRoomCount = (short)(GameManager.worldSize.x * GameManager.worldSize.y);
        short remainingRooms = totalRoomCount;
        roomTypeCounts[(short)RoomType.empty] = (short)Mathf.FloorToInt(totalRoomCount * .75f);
        remainingRooms -= roomTypeCounts[(short)RoomType.empty];
        roomTypeCounts[(short)RoomType.trapGas] = (short)Mathf.FloorToInt(totalRoomCount * .05f);
        remainingRooms-= roomTypeCounts[(short)RoomType.trapGas];
        roomTypeCounts[(short)RoomType.trapSpike] = (short)Mathf.FloorToInt(totalRoomCount * .1f);
        remainingRooms -= roomTypeCounts[(short)RoomType.trapSpike];
        roomTypeCounts[(short)RoomType.trapMonster] = (short)Mathf.FloorToInt(totalRoomCount * .05f);
        remainingRooms -= roomTypeCounts[(short)RoomType.trapMonster];
        roomTypeCounts[(short)RoomType.treasure] = (short)Mathf.FloorToInt(totalRoomCount * .05f);
        remainingRooms -= roomTypeCounts[(short)RoomType.treasure];

        roomTypeCounts[(short)RoomType.trapSpike] += remainingRooms;
    }

    private RoomType[,] GenerateRooms (){
        bool [,] trappedRoomArray = new bool[GameManager.worldSize.x, GameManager.worldSize.y];

        // Randomly set trapped rooms
        for (int i = 0; i < trappedRoomsCount; i++)
        {
            int x, y;

            do
            {
                x = Random.Range(0, GameManager.worldSize.x);
                y = Random.Range(0, GameManager.worldSize.y);
            } while (trappedRoomArray[x, y] || IsNearStartingOrEndPosition(x, y));

            trappedRoomArray[x, y] = true;
            UpdateDangerLevelArray(new Vector2Int(x, y));
        }

        return GenerateRoomTypes(trappedRoomArray);
    }

    private RoomType[,] GenerateRoomTypes (bool[,] trapRoomArray){
        RoomType[,] roomTypeArray = new RoomType[GameManager.worldSize.x, GameManager.worldSize.y];
        short[] availableRoomTypes = roomTypeCounts;
        for (int x = 0; x < GameManager.worldSize.x; x++){
            for (int y = 0; y < GameManager.worldSize.y; y++){
                if (trapRoomArray[x,y]){
                    roomTypeArray[x,y] = GetRandomRoomType(availableRoomTypes);
                    availableRoomTypes[(int)roomTypeArray[x,y]]--;
                }else{
                    roomTypeArray[x,y] = RoomType.empty;
                }
            }
        }
        return roomTypeArray;
    }

    private RoomType GetRandomRoomType(short[] availableRoomTypes){
        short roomsLeft = 0;

        for (int i = 1; i < availableRoomTypes.Length; i++){
            roomsLeft += availableRoomTypes[i];
        }

        int value = Random.Range(0, roomsLeft);

        for (int i = 1; i < availableRoomTypes.Length; i++){
            if (value > availableRoomTypes[i]){
                value -= availableRoomTypes[i];
            }else{
                return (RoomType)i;
            }
            
        }
        return RoomType.empty;
    }

    private bool IsNearStartingOrEndPosition(int x, int y)
    {
        int proximity = 1;

        return (Mathf.Abs(x - startingPosition.x) <= proximity &&
            Mathf.Abs(y - startingPosition.y) <= proximity) ||
            (Mathf.Abs(x - endPosition.x) <= proximity &&
            Mathf.Abs(y - endPosition.y) <= proximity);
    }

    private void UpdateDangerLevelArray (Vector2Int gridPosition){
        for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++){
            for (int y = gridPosition.y - 1; y <= gridPosition.y + 1; y++){
                if (x < 0 || x >= GameManager.worldSize.x || y < 0 || y >= GameManager.worldSize.y || (x == gridPosition.x && y == gridPosition.y)) continue;
                dangerLevelArray[x,y]++;
            }
        }
    }

    private void GenerateRoomStructures (RoomType[,] roomTypeArray){
        Room[,] roomArray = new Room[GameManager.worldSize.x, GameManager.worldSize.y];
        for (int x = 0; x < GameManager.worldSize.x; x++){
            for (int y = 0; y < GameManager.worldSize.y; y++){
                Vector3 position = new Vector3(x * roomDistance, 0f, y * roomDistance);
                GameObject room = Instantiate(GetRandomRoomPrefab(roomTypeArray[x,y]), position, Quaternion.identity);
                room.transform.parent = roomManager;
                roomArray[x,y] = room.GetComponent<Room>();
                roomArray[x,y].InitializeRoom(new Vector2Int(x,y), roomTypeArray[x,y], dangerLevelArray[x,y], false);
            }
        }

        RoomManager.Instance.SetUpRoomManager(roomArray);
    }

    private GameObject GetRandomRoomPrefab (RoomType roomType){
        switch (roomType){
            case RoomType.empty:
                return emptyRoomPrefabs[0];
            case RoomType.trapSpike:
                return spikeTrapRoomPrefabs[0];
            case RoomType.trapGas:
                return gasTrapRoomPrefabs[0];
            case RoomType.trapMonster:
                return monsterTrapRoomPrefab[0];
            case RoomType.treasure:
                return treasureRoomPrefabs[0];
            default:
                return defaultRoomPrefab;
        }
    }
}


