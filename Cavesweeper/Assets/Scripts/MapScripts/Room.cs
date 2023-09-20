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

    #region Room Interactions

    public void UnlockRoom (bool forceUnlockSurroundingRooms = false){
        if (isOpen) return;
        // if (roomType == RoomType.trapGas){
        //     UnlockRoom(true);
        //     return;
        // }
        isOpen = true;

        List<RoomDirection> roomDirections = RoomManager.Instance.GetOpenNeighbourRoomDirections(gridPosition);

        foreach (RoomDirection roomDirection in roomDirections){
            switch (roomDirection){
                case RoomDirection.north:
                    if (roomWalls[(int)Walls.north].tag != "EdgeWall" && RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.north))
                    Destroy(roomWalls[(int)Walls.north]);
                break;

                case RoomDirection.south:
                    if (roomWalls[(int)Walls.south].tag != "EdgeWall" && RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.south))
                    Destroy(roomWalls[(int)Walls.south]);
                break;

                case RoomDirection.west:
                    if (roomWalls[(int)Walls.west].tag != "EdgeWall" && RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.west))
                    Destroy(roomWalls[(int)Walls.west]);
                break;

                case RoomDirection.east:
                    if (roomWalls[(int)Walls.east].tag != "EdgeWall" && RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.east))
                    Destroy(roomWalls[(int)Walls.east]);
                break;

                case RoomDirection.northWest:
                    if (roomWalls[(int)Walls.north].tag != "EdgeWall" && 
                        RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.north)) Destroy(roomWalls[(int)Walls.north]);
                    if (roomWalls[(int)Walls.west].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.west)) Destroy(roomWalls[(int)Walls.west]);
                    if (RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northWest) &&
                        roomCorners[(int)Corners.northWest].tag != "SafePillar" && roomCorners[(int)Corners.northWest].tag != "EdgePillar") 
                    Destroy(roomCorners[(int)Corners.northWest]);
                break;

                case RoomDirection.northEast:
                    if (roomWalls[(int)Walls.north].tag != "EdgeWall" && 
                        RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.north)) Destroy(roomWalls[(int)Walls.north]);
                    if (roomWalls[(int)Walls.east].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.east)) Destroy(roomWalls[(int)Walls.east]);
                    if (RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.northEast) &&
                        roomCorners[(int)Corners.northEast].tag != "SafePillar" && roomCorners[(int)Corners.northEast].tag != "EdgePillar") 
                    Destroy(roomCorners[(int)Corners.northEast]);
                break;

                case RoomDirection.southWest:
                    if (roomWalls[(int)Walls.south].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.south)) Destroy(roomWalls[(int)Walls.south]);
                    if (roomWalls[(int)Walls.west].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.west)) Destroy(roomWalls[(int)Walls.west]);
                    if (RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southWest) &&
                        roomCorners[(int)Corners.southWest].tag != "SafePillar" && roomCorners[(int)Corners.southWest].tag != "EdgePillar") 
                    Destroy(roomCorners[(int)Corners.southWest]);
                break;

                case RoomDirection.southEast:
                    if (roomWalls[(int)Walls.south].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.south)) Destroy(roomWalls[(int)Walls.south]);
                    if (roomWalls[(int)Walls.east].tag != "EdgeWall" && 
                    RoomManager.Instance.IsWallValidForDestruction(gridPosition, Walls.east)) Destroy(roomWalls[(int)Walls.east]);
                    if (RoomManager.Instance.IsCornerValidForDestruction(gridPosition, Corners.southEast) &&
                        roomCorners[(int)Corners.southEast].tag != "SafePillar" && roomCorners[(int)Corners.southEast].tag != "EdgePillar") 
                    Destroy(roomCorners[(int)Corners.southEast]);
                break;
            }
        }

        if (dangerLevel == 0 || forceUnlockSurroundingRooms){
            RoomManager.Instance.ClearSurroundingRooms(gridPosition);
            return;
        }
    }

    #endregion

    #region Getters

    public bool IsOpen (){
        return isOpen;
    }

    public RoomDirection GetRoomDirection (Vector2Int direction){
        direction = gridPosition - direction;

        if (direction.x < 0){
            if (direction.y < 0){
                return RoomDirection.southWest;
            }
            else if (direction.y > 0){
                return RoomDirection.northWest;
            } 
            else{
                return RoomDirection.west;
            }
        }
        else if (direction.x > 0){
            if (direction.y < 0){
                return RoomDirection.southEast;
            }
            else if (direction.y > 0){
                return RoomDirection.northEast;
            } 
            else{
                return RoomDirection.east;
            }
        }else{
            if (direction.y > 0){
                return RoomDirection.north;
            }else{
                return RoomDirection.south;
            }
        }
    }

    #endregion
}
