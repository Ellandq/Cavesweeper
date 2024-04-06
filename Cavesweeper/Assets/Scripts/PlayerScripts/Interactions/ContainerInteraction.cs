using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInteraction : EntityInteraction
{
    [SerializeField] private ItemHolder itemHolder;

    private void OnValidate()
    {
        if (interactionName == "Open ") return;
        interactionName = "Open ";
        interactionType = InteractionType.Container;
        itemHolder = GetComponentInParent<ItemHolder>();
        gameObject.tag = "Interactable";
    }
    
    public override void Interact(Player player)
    {
        Debug.Log("Interacting with container");
        // player.GetInventory().AddItems(itemHolder.GetItems());
        //
        // Destroy(itemHolder.gameObject);
    }
}
