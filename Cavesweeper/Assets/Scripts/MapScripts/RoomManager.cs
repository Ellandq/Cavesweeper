using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header ("Rooms Information")]
    private Room[,] roomGrid;

    private void Awake (){
        Instance = this;
    }

   public void SetUpRoomManager (Room[,] roomGrid){
        this.roomGrid = roomGrid;
   }
}

public enum RoomType{
    empty, 
    trapSpike, trapGas, trapMonster,
    treasure
}
