using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header ("Events")]
    private Dictionary<string, Action> OnButtonDown;
    private Dictionary<string, Action> OnButtonUp;

    [Header ("Key Information")]
    private Dictionary<string, KeyCode> buttonAssignment;
    private Dictionary<KeyCode, bool> buttonStates;

    private void Awake ()
    {
        buttonStates = new Dictionary<KeyCode, bool>();
        buttonAssignment = InputManager.defaultInputCodes.Zip(InputManager.defaultInputKeys, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateButtonStateDictionary();
        UpdateEventDictionaries();
    }
    
    private void OnApplicationQuit()
    {
        OnButtonDown.Clear();
        OnButtonUp.Clear();
    }

    private void Update ()
    {
        foreach (KeyValuePair<string, KeyCode> button in buttonAssignment)
        {
            bool previousState = buttonStates[button.Value];

            buttonStates[button.Value] = Input.GetKey(button.Value);

            if (buttonStates[button.Value] && !previousState)
            {
                OnButtonDown[button.Key]?.Invoke();
            }
            else if (!buttonStates[button.Value] && previousState) 
            {
                OnButtonUp[button.Key]?.Invoke();
            }
        }
    } 

    public void AssignButtons (string buttonName, KeyCode buttonKey)
    {
        buttonAssignment.Add(buttonName, buttonKey);
        UpdateButtonStateDictionary ();
    }

    public void AssignButtons (Dictionary<string, KeyCode> buttonAssignment)
    {
        this.buttonAssignment = buttonAssignment;
        UpdateButtonStateDictionary ();
    }

    private void UpdateButtonStateDictionary () 
    {
        buttonStates = new Dictionary<KeyCode, bool>();

        foreach (KeyValuePair<string, KeyCode> button in buttonAssignment){
            buttonStates.Add(button.Value, false);
        }
    }

    private void UpdateEventDictionaries () 
    {
        OnButtonDown = new Dictionary<string, Action>();
        OnButtonUp = new Dictionary<string, Action>();

        foreach (string action in InputManager.defaultInputCodes){
            OnButtonDown.Add(action, () => {});
            OnButtonUp.Add(action, () => {});
        }
    }
    
    public void AddListenerOnButtonDown (Action actionToAdd, string key) { OnButtonDown[key] += actionToAdd; }
    
    public void AddListenerOnButtonUp (Action actionToAdd, string key) { OnButtonUp[key] += actionToAdd; }

    public bool GetButtonState (KeyCode key){
        return buttonStates[key];
    }
}
