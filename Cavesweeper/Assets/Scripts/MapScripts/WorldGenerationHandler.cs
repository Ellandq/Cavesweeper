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
    [SerializeField] private float roomDistance;

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

    [Header ("Wall and Corner Prefabs")]
    [SerializeField] private List<GameObject> wallPrefabs;
    [SerializeField] private List<GameObject> cornerPrefabs;

    [Header ("Object References")]
    [SerializeField] private Transform roomManager;

    private void Awake (){
        Instance = this;
    }

    private void FinishWorldGeneration (){
        RoomManager.Instance.ClearSurroundingRooms(startingPosition);
        // RoomManager.Instance.ClearSurroundingRooms(endPosition);
        GameManager.Instance.SetStartingPlayerPosition(GetWorldPositionFromRoomPosition(startingPosition));
    }

    #region World Generation

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

        FinishWorldGeneration();
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
        GameObject[,] cornerArray = GenerateRoomCorners();
        GameObject[,] wallArray = GenerateRoomWalls();

        Room[,] roomArray = new Room[GameManager.worldSize.x, GameManager.worldSize.y];
        for (int x = 0; x < GameManager.worldSize.x; x++){
            for (int y = 0; y < GameManager.worldSize.y; y++){
                Vector3 position = new Vector3(x * roomDistance, 0f, y * roomDistance);
                GameObject room = Instantiate(GetRandomRoomPrefab(roomTypeArray[x,y]), position, Quaternion.identity);
                room.transform.parent = roomManager;
                roomArray[x,y] = room.GetComponent<Room>();
                roomArray[x,y].InitializeRoom(new Vector2Int(x,y), roomTypeArray[x,y], dangerLevelArray[x,y], GetRoomWalls(x, y, wallArray), GetRoomCorners(x, y, cornerArray), false);
            }
        }

        RoomManager.Instance.SetUpRoomManager(roomArray);
    }

    private GameObject GetRandomRoomPrefab (RoomType roomType){
        switch (roomType){
            case RoomType.empty:
                return emptyRoomPrefabs[Random.Range(0, emptyRoomPrefabs.Count)];
            case RoomType.trapSpike:
                return spikeTrapRoomPrefabs[Random.Range(0, spikeTrapRoomPrefabs.Count)];
            case RoomType.trapGas:
                return gasTrapRoomPrefabs[Random.Range(0, gasTrapRoomPrefabs.Count)];
            case RoomType.trapMonster:
                return monsterTrapRoomPrefab[Random.Range(0, monsterTrapRoomPrefab.Count)];
            case RoomType.treasure:
                return treasureRoomPrefabs[Random.Range(0, treasureRoomPrefabs.Count)];
            default:
                return defaultRoomPrefab;
        }
    }

    private GameObject[,] GenerateRoomCorners (){
        GameObject[,] cornerArray = new GameObject[GameManager.worldSize.x + 1, GameManager.worldSize.y + 1];
        float offset = -(roomDistance / 2);
        for (int x = 0; x <= GameManager.worldSize.x; x++){
            for (int y = 0; y <= GameManager.worldSize.y; y++){
                Vector3 position = new Vector3((x - 1) * roomDistance - offset, 0f, (y - 1) * roomDistance - offset);
                
                cornerArray[x, y] = Instantiate(GetCornerType(x, y), position, Quaternion.identity);
                cornerArray[x, y].transform.parent = roomManager.GetChild(0);
                cornerArray[x, y].name = "Corner ( X: " + x +", Y: " + y + " )";
            }
        }

        return cornerArray;
    }

    private GameObject GetCornerType (int x, int y){
        if (x == 0 || x == GameManager.worldSize.x ||
            y == 0 || y == GameManager.worldSize.y){
            return cornerPrefabs[0];
        }else if (dangerLevelArray[x, y] == 0 && dangerLevelArray[x, y - 1] == 0 &&
                  dangerLevelArray[x - 1, y] == 0 && dangerLevelArray[x - 1, y - 1] == 0){
            return cornerPrefabs[1];
        }else{
            return cornerPrefabs[2];
        }
    }

    private GameObject[,] GenerateRoomWalls (){
        GameObject[,] wallArray = new GameObject[GameManager.worldSize.x * 2 + 1, GameManager.worldSize.y * 2 + 1];
        for (int x = 0; x <= GameManager.worldSize.x * 2; x++){
            for (int y = 0; y <= GameManager.worldSize.y * 2; y++){
                Vector3 position = new Vector3(Mathf.FloorToInt(x / 2) * roomDistance, 0f, Mathf.FloorToInt(y / 2) * roomDistance);

                if (y % 2 == 0){
                    if (x == GameManager.worldSize.x) continue;

                    wallArray[x, y] = Instantiate(GetWallType(x, y), position, Quaternion.Euler(0f, 0f, 0f));
                    wallArray[x, y].transform.parent = roomManager.GetChild(1);
                    wallArray[x, y].name = "Wall ( X: " + x +", Y: " + y + " )";
                }else{
                    wallArray[x, y] = Instantiate(GetWallType(x, y), position, Quaternion.Euler(0f, 90f, 0f));
                    wallArray[x, y].transform.parent = roomManager.GetChild(1);
                    wallArray[x, y].name = "Wall ( X: " + x +", Y: " + y + " )";
                }
                
            }
        }
        return wallArray;
    }

    private GameObject GetWallType (int x, int y){
        
        if (    x == 0
            ||  y == 0
            ||  x == GameManager.worldSize.x + 1
            ||  y == GameManager.worldSize.y * 2 + 1){
            return wallPrefabs[0];
        }else{
            return wallPrefabs[1];
        }
    }

    #endregion

    #region Getters

    public Vector3 GetWorldPositionFromRoomPosition (Vector2Int roomPosition){
        return new Vector3(roomPosition.x * roomDistance, -1.52f, roomPosition.y * roomDistance);
    }

    private List<GameObject> GetRoomCorners (int x, int y, GameObject[,] cornerArray){
        List<GameObject> cornerList = new List<GameObject>(){
            cornerArray[x, y],
            cornerArray[x + 1, y],
            cornerArray[x, y + 1],
            cornerArray[x + 1, y + 1]
        };

        return cornerList;
    }

    private List<GameObject> GetRoomWalls (int x, int y, GameObject[,] wallArray){
        List<GameObject> wallList = new List<GameObject>(){
            wallArray[x, y],
            wallArray[x, y + 1],
            wallArray[x + 1, y + 1],
            wallArray[x, y + 2]
        };
        return wallList;
    }

    #endregion
}


