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

    #region Room Interactions
    
    public void ClearSurroundingRooms(Vector2Int gridPosition){
        List<Room> neighbourList = GetNeighbourRooms(gridPosition);
        
        foreach (Room room in neighbourList){
            room.UnlockRoom(gridPosition);
        }
    }

    #endregion

    #region Getters

    public Room GetRoom (Vector2Int gridPosition){
        return roomGrid[gridPosition.x, gridPosition.y];
    }

    public List<Room> GetNeighbourRooms (Vector2Int gridPosition){
        List<Room> neighbourList = new List<Room>();
        for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++){
            for (int y = gridPosition.y - 1; y <= gridPosition.y + 1; y++){
                if ((x == gridPosition.x && y == gridPosition.y) 
                    || x < 0 || x >= GameManager.worldSize.x
                    || y < 0 || y >= GameManager.worldSize.y) continue;
                    
                neighbourList.Add(roomGrid[x,y]);
            }
        }
        return neighbourList;
    }

    public bool IsCornerValidForDestruction (Vector2Int roomPosition, Corners cornerDirection){
        switch (cornerDirection){
            case Corners.southWest:
                if (roomGrid[roomPosition.x - 1, roomPosition.y].IsOpen() 
                &&  roomGrid[roomPosition.x, roomPosition.y - 1].IsOpen() 
                &&  roomGrid[roomPosition.x - 1, roomPosition.y - 1].IsOpen() )
                return true;
                else return false;
            
            case Corners.southEast:
                if (roomGrid[roomPosition.x + 1, roomPosition.y].IsOpen() 
                &&  roomGrid[roomPosition.x, roomPosition.y - 1].IsOpen() 
                &&  roomGrid[roomPosition.x + 1, roomPosition.y - 1].IsOpen() )
                return true;
                else return false;

            case Corners.northWest:
                if (roomGrid[roomPosition.x - 1, roomPosition.y].IsOpen() 
                &&  roomGrid[roomPosition.x, roomPosition.y + 1].IsOpen() 
                &&  roomGrid[roomPosition.x - 1, roomPosition.y + 1].IsOpen() )
                return true;
                else return false;

            case Corners.northEast:
                if (roomGrid[roomPosition.x + 1, roomPosition.y].IsOpen() 
                &&  roomGrid[roomPosition.x, roomPosition.y + 1].IsOpen() 
                &&  roomGrid[roomPosition.x + 1, roomPosition.y + 1].IsOpen() )
                return true;
                else return false;              
            
            default:
            return false;
        }
    }

    #endregion
}

public enum RoomType{
    empty, 
    trapSpike, trapGas, trapMonster,
    treasure
}

public enum Corners{
    southWest, southEast,
    northWest, northEast
}

public enum Walls{
    south, west,
    east, north
}