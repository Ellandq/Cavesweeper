using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header ("Game Information")]
    public static int worldSeed;
    public static Vector2Int worldSize;

    [Header ("Object References")]
    [SerializeField] private GameObject player;

    private void Awake (){
        Instance = this;
    }

    private void Start (){
        worldSeed = Random.Range(0, 100000);
        worldSize = new Vector2Int(30, 16);

        WorldGenerationHandler.Instance.GenerateWorld(99);
    }

    public void SetStartingPlayerPosition (Vector3 position){
        player.transform.position = position;
    }
}
