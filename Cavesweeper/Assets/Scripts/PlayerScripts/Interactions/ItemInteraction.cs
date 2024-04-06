using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : EntityInteraction
{

    [SerializeField] private ItemHolder itemHolder;
    
    private void OnValidate()
    {
        if (interactionName == "Pick up ") return;
        interactionName = "Pick up ";
        interactionType = InteractionType.Item;
        itemHolder = GetComponentInParent<ItemHolder>();
        gameObject.tag = "Interactable";
    }
    
    public override void Interact(Player player)
    {
        player.GetInventory().AddItems(itemHolder.GetItems());
        
        Destroy(itemHolder.gameObject);
    }
}
