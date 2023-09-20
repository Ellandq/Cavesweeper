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
            room.UnlockRoom();
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

    public List<RoomDirection> GetOpenNeighbourRoomDirections (Vector2Int gridPosition){
        List<Room> neighbourRooms = GetNeighbourRooms(gridPosition);
        List<RoomDirection> directionList = new List<RoomDirection>();

        foreach (Room room in neighbourRooms){
            if (!room.IsOpen()) continue;
            directionList.Add(room.GetRoomDirection(gridPosition));
        }
        return directionList;
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

    public bool IsWallValidForDestruction (Vector2Int roomPosition, Walls wallDirection){
        switch (wallDirection){
            case Walls.south:
                return GetRoom(new Vector2Int(roomPosition.x, roomPosition.y - 1)).IsOpen();
            case Walls.north:
                return GetRoom(new Vector2Int(roomPosition.x, roomPosition.y + 1)).IsOpen();
            case Walls.west:
                return GetRoom(new Vector2Int(roomPosition.x - 1, roomPosition.y)).IsOpen();
            case Walls.east:
                return GetRoom(new Vector2Int(roomPosition.x + 1, roomPosition.y)).IsOpen();
            default: 
                return false;
        }
    }

    #endregion
}

// public static class RoomDirection{
//     public static readonly Vector2Int north = new Vector2Int(0, 1);
//     public static readonly Vector2Int south = new Vector2Int(0, -1);
//     public static readonly Vector2Int west = new Vector2Int(-1, 0);
//     public static readonly Vector2Int east = new Vector2Int(1, 0);
//     public static readonly Vector2Int northWest = new Vector2Int(-1, 1);
//     public static readonly Vector2Int northEast = new Vector2Int(1, 1);
//     public static readonly Vector2Int southWest = new Vector2Int(-1, -1);
//     public static readonly Vector2Int southEast = new Vector2Int(1, -1);
// }

public enum RoomType{
    empty, 
    trapSpike, trapGas, trapMonster,
    treasure
}

public enum RoomDirection{
    north, south,
    west, east,
    northWest, northEast,
    southWest, southEast
}

public enum Corners{
    southWest, southEast,
    northWest, northEast
}

public enum Walls{
    south, west,
    east, north
}