using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    [Header ("Room Information")]
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private RoomType roomType;
    private byte dangerLevel;
    private bool isOpen;

    [Header ("Object References")]
    [SerializeField] private TMP_Text dangerLevelDisplay;

    public void InitializeRoom (Vector2Int gridPosition, RoomType roomType, byte dangerLevel, bool isOpen = false){
        this.roomType = roomType;
        this.gridPosition = gridPosition;
        this.dangerLevel = dangerLevel;
        this.isOpen = isOpen;

        dangerLevelDisplay.text = dangerLevel.ToString();
        this.gameObject.name = "Room ( X: " + gridPosition.x +", Y: " + gridPosition.y + " )";
    }
}
