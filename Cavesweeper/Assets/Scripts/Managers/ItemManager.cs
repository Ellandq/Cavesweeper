using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Item Prefabs")] 
    [SerializeField] private List<GameObject> containers;
    
    [ContextMenu("Set Instance")]
    private void SetInstance()
    {
        Instance = this;
    }
}