using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationHandler : MonoBehaviour
{
    [Header ("World Information")]
    private int worldSeed;
    private Vector2Int worldSize;
    private int trappedRoomsCount;
    private Vector2Int startingPosition;
    private Vector2Int endPosition;

    //[Header ("Rooms information")]

    [Header ("Object References")]
    [SerializeField] private GameObject defaultRoomPrefab;
    [SerializeField] private List<GameObject> emptyRoomPrefabs;
    [SerializeField] private List<GameObject> trappedRoomPrefabs;
    [SerializeField] private List<GameObject> prizeRoomPrefabs;

    public void GenerateWorld (int worldSeed, Vector2Int worldSize, int trappedRoomsCount){
        this.worldSeed = worldSeed;
        this.worldSize = worldSize;
        this.trappedRoomsCount = trappedRoomsCount;

        Random.InitState(worldSeed);
    }
}
