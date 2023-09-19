using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    [Header ("Room Information")]
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private RoomType roomType;
    [SerializeField] private byte dangerLevel;
    [SerializeField] private bool isOpen = false;

    [Header ("Object References")]
    [SerializeField] private TMP_Text dangerLevelDisplay;
    // Walls
    [SerializeField] private List<GameObject> roomWalls;
    // Corners
    [SerializeField] private List<GameObject> roomCorners;


    public void InitializeRoom (Vector2Int gridPosition, RoomType roomType, byte dangerLevel, List<GameObject> wallList, List<GameObject> cornerList, bool isOpen = false){
        this.roomType = roomType;
        this.gridPosition = gridPosition;
        this.dangerLevel = dangerLevel;
        this.isOpen = isOpen;
    
        roomWalls.AddRange(wallList);
        roomCorners.AddRange(cornerList);

        dangerLevelDisplay.text = dangerLevel.ToString();
        this.gameObject.name = "Room ( X: " + gridPosition.x +", Y: " + gridPosition.y + " )";
    }

    public void UnlockRoom (Vector2Int direction){
        isOpen = true;
        direction -= gridPosition;

        if (direction.x != 0 && direction.y != 0){
            if (direction.x < 0 && direction.y < 0){
                if (roomWalls[(int)Walls.south].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.south]);
                if (roomWalls[(int)Walls.west].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.west]);
                if (roomCorners[(int)Corners.southWest].tag != "SafePillar") Destroy(roomCorners[(int)Corners.southWest]);
            }
            else if (direction.x > 0 && direction.y < 0){
                if (roomWalls[(int)Walls.south].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.south]);
                if (roomWalls[(int)Walls.east].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.east]);
                if (roomCorners[(int)Corners.southEast].tag != "SafePillar") Destroy(roomCorners[(int)Corners.southEast]);
            }
            else if (direction.x < 0 && direction.y > 0){
                if (roomWalls[(int)Walls.north].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.north]);
                if (roomWalls[(int)Walls.west].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.west]);
                if (roomCorners[(int)Corners.northWest].tag != "SafePillar") Destroy(roomCorners[(int)Corners.northWest]);
            }
            else if (direction.x > 0 && direction.y > 0){
                if (roomWalls[(int)Walls.north].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.north]);
                if (roomWalls[(int)Walls.east].tag != "EdgeWall") Destroy(roomWalls[(int)Walls.east]);
                if (roomCorners[(int)Corners.northEast].tag != "SafePillar") Destroy(roomCorners[(int)Corners.northEast]);
            }
        }
        else if (direction.x < 0){
            Destroy(roomWalls[(int)Walls.west]);
            if (roomCorners[(int)Corners.northWest].tag != "EdgePillar" && 
            RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northWest)){   
                Destroy(roomCorners[(int)Corners.northWest]);
            }
            if (roomCorners[(int)Corners.southWest].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southWest)){   
                Destroy(roomCorners[(int)Corners.southWest]);
            }
        }
        else if (direction.x > 0){
            Destroy(roomWalls[(int)Walls.east]);
            if (roomCorners[(int)Corners.northEast].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northEast)){   
                Destroy(roomCorners[(int)Corners.northEast]);
            }
            if (roomCorners[(int)Corners.southEast].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southEast)){   
                Destroy(roomCorners[(int)Corners.southEast]);
            }
        }
        else if (direction.y < 0){
            Destroy(roomWalls[(int)Walls.south]);
            if (roomCorners[(int)Corners.southWest].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southWest)){   
                Destroy(roomCorners[(int)Corners.southWest]);
            }
            if (roomCorners[(int)Corners.southEast].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southEast)){   
                Destroy(roomCorners[(int)Corners.southEast]);
            }
        }
        else if (direction.y > 0){
            Destroy(roomWalls[(int)Walls.north]);
            if (roomCorners[(int)Corners.northWest].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northWest)){   
                Destroy(roomCorners[(int)Corners.northWest]);
            }
            if (roomCorners[(int)Corners.northEast].tag != "EdgePillar" && 
                RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northEast)){   
                Destroy(roomCorners[(int)Corners.northEast]);
            }
        }
    }

    public void UnlockRoom (){
        isOpen = true;
        RoomManager.Instance.ClearSurroundingRooms(gridPosition);
    }

    #region Getters

    public bool IsOpen (){
        return isOpen;
    }

    #endregion
}
