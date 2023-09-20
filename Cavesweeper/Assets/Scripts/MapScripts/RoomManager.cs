using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
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
    
    public void ClearRoom (string roomName){
        GetRoomFromWallPosition(ExtractCoordinates(roomName)).UnlockRoom();
    }

    public void ClearSurroundingRooms(Vector2Int gridPosition){
        GetRoom(gridPosition).UnlockRoom();
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

    private Room GetRoomFromWallPosition (Vector2Int wallPosition){
        Vector2Int position = new Vector2Int();

        if (wallPosition.y % 2 == 1){
            position.x = wallPosition.x - 1;
            position.y = (wallPosition.y - 1) / 2;

            if (GetRoom(position).IsOpen()){
                position.x++;
                return GetRoom(position);
            }else{
                return GetRoom(position);
            }
        }else{
            position.x = wallPosition.x;
            position.y = wallPosition.y / 2;

            if (GetRoom(position).IsOpen()){
                position.y--;
                return GetRoom(position);
            }else{
                return GetRoom(position);
            }
        }
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

    public Vector2Int ExtractCoordinates(string objectName)
    {
        string pattern = @"Wall \( X: (\d+), Y: (\d+) \)";
        Match match = Regex.Match(objectName, pattern);

        if (match.Success && match.Groups.Count == 3)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);
            return new Vector2Int(x, y);
        }
        else
        {
            return Vector2Int.one * 2;
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