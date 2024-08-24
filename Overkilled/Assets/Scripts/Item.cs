using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] ItemSO _itemInfo;

    public ItemSO GetItemInfo() { return _itemInfo; }

    public void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetComponent<PlayerHand>();
        if (hand != null)
        {
            hand.SetItem(this);
        }
    }
}
