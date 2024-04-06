using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header ("Input Handles")]
    [SerializeField] private PlayerInput inputHandle;
    [SerializeField] private MouseInput mouseInputHandle;

    public void Awake () 
    { 
        if  (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }else{
            Destroy(gameObject);
        }
    }

    public static PlayerInput GetInputHandle (){
        return Instance.inputHandle;
    }


    public static MouseInput GetMouseHandle (){
        return Instance.mouseInputHandle;
    }

    public static List<string> defaultInputCodes = new List<string>(){
        "Move Left", "Move Right", "Move Forwards", "Move Backwards", 
        "Jump", "Sprint", "Crouch", "Walk", "Interact", "Inventory", "Character Info", 
        "Item Slot 1", "Item Slot 2", "Item Slot 3", "Item Slot 4", "Item Slot 5",
    };

    public static List<KeyCode> defaultInputKeys = new List<KeyCode>(){
        KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
        KeyCode.Space, KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.E, KeyCode.Tab, KeyCode.C,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
    };

    public static List<string> defaultMouseInput = new List<string>(){
        "Base", "Alternative", 
        // "Special 01", "Special 02", "Special 03", "Special 04", "Special 05"
    };

    public static List<int> defaultInputIndexes = new List<int>(){
        0, 1
    };
}

