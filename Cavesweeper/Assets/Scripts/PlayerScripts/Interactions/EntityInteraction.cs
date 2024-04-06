using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInteraction : MonoBehaviour
{
    [Header("Interaction Settings")] 
    [SerializeField] protected InteractionType interactionType;
    [SerializeField] protected string interactionName;

    public virtual void Interact(Player player)
    {
        Debug.LogError("Interaction not specified on object: " + gameObject.name);
    }

    public InteractionType GetInteractionType() { return interactionType; }

    public string GetInteractionName() { return interactionName; }
    
}

public enum InteractionType
{
    Item, Container, 
    
    Door, Enterance,
    
    Npc,
    
    Light
}
