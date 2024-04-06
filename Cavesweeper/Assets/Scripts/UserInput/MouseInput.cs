using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseInput : MonoBehaviour
{
    [Header ("Events")]
    private Dictionary<string, Action> OnButtonDown;
    private Dictionary<string, Action> OnButtonUp;
    private Action<EntityInteraction> OnSelectedObjectChange;

    [Header ("Key Information")]
    private Dictionary<string, int> buttonAssignment;
    private Dictionary<int, bool> buttonStates;

    [Header ("Raycast Settings")]
    private Camera playerCamera;
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;

    [Header ("Object Selection")]
    [SerializeField] private EntityInteraction selectedObject;

    private void Awake ()
    {
        buttonStates = new Dictionary<int, bool>();
        buttonAssignment = new Dictionary<string, int>();
        buttonAssignment = InputManager.defaultMouseInput.Zip(InputManager.defaultInputIndexes, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateButtonStateDictionary();
        UpdateEventDictionaries();
    }

    private void Start ()
    {
        playerCamera = CameraManager.GetCurrentCamera();
    }

    private void OnApplicationQuit()
    {
        OnButtonDown.Clear();
        OnButtonUp.Clear();
        OnSelectedObjectChange = null;
    }

    private void Update ()
    {
        foreach (KeyValuePair<string, int> button in buttonAssignment)
        {
            bool previousState = buttonStates[button.Value];

            buttonStates[button.Value] = Input.GetMouseButton(button.Value);

            if (buttonStates[button.Value] && !previousState)
            {
                OnButtonDown[button.Key]?.Invoke();
            }
            else if (!buttonStates[button.Value] && previousState) 
            {
                OnButtonUp[button.Key]?.Invoke();
            }
        }

        CheckForObjects();
    } 

    private void CheckForObjects ()
    {
        var ray = playerCamera.ScreenPointToRay(playerCamera.pixelRect.center);

        if (Physics.Raycast(ray, out var hit, range, layerMask))
        {
            GameObject obj = hit.collider.transform.gameObject;

            if (!obj.CompareTag("Interactable")) return;
            var interactor = obj.GetComponent<EntityInteraction>();
            
            if (interactor == selectedObject) return;
            selectedObject = interactor;

            OnSelectedObjectChange?.Invoke(selectedObject);
        }
        else if (!ReferenceEquals(selectedObject, null))
        {
            selectedObject = null;
            OnSelectedObjectChange?.Invoke(selectedObject);
        }
    }

    public void CheckForObjectRemoval (EntityInteraction obj)
    {
        if (obj != selectedObject) return;
        
        selectedObject = null;
        OnSelectedObjectChange?.Invoke(selectedObject);
    }

    private void UpdateCamera (Camera camera)
    {
        playerCamera = camera;
    }

    public void AssignButtons (string buttonName, int buttonKey)
    {
        buttonAssignment.Add(buttonName, buttonKey);
        UpdateButtonStateDictionary ();
    }

    public void AssignButtons (Dictionary<string, int> buttonAssignment)
    {
        this.buttonAssignment = buttonAssignment;
        UpdateButtonStateDictionary ();
    }

    private void UpdateButtonStateDictionary () 
    {
        buttonStates = new Dictionary<int, bool>();

        foreach (KeyValuePair<string, int> button in buttonAssignment){
            buttonStates.Add(button.Value, false);
        }
    }

    private void UpdateEventDictionaries () 
    {
        OnButtonDown = new Dictionary<string, Action>();
        OnButtonUp = new Dictionary<string, Action>();

        foreach (var action in InputManager.defaultMouseInput){
            OnButtonDown.Add(action, () => {});
            OnButtonUp.Add(action, () => {});
        }
    }

    public void AddListenerOnButtonDown (Action actionToAdd, string key) { OnButtonDown[key] += actionToAdd; }
    
    public void AddListenerOnButtonUp (Action actionToAdd, string key) { OnButtonUp[key] += actionToAdd; }
    
    public void AddListenerOnObjectChange (Action<EntityInteraction> actionToAdd) { OnSelectedObjectChange += actionToAdd; }

    public EntityInteraction GetSelectedObject () { return selectedObject; }

    public bool GetButtonState (int key){
        return buttonStates[key];
    }
    
    
}
