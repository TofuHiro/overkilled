using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour, IInteractable
{
    public virtual void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetHand();

        if (!hand.IsHoldingItem)
            return;

        DestroyItemFromHand(hand);
    }

    void DestroyItemFromHand(PlayerHand hand)
    {
        Item item = hand.GetItem();
        hand.ReleaseItem();
        ObjectSpawner.Instance.DestroyObject(item.gameObject);
    }
}
