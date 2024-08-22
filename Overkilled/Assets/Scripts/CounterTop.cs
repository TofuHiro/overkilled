using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CounterTop : MonoBehaviour, IInteractable
{
    [SerializeField] Transform _itemHoldersParent;

    protected ItemHolder[] _holders;
    int _itemHolderCount;

    public bool CounterIsFull
    {
        get
        {
            foreach (ItemHolder holder in _holders)
                if (!holder.IsOccupied)
                    return false;
            return true;
        }
    }

    public bool CounterIsEmpty
    {
        get
        {
            foreach (ItemHolder holder in _holders)
                if (holder.IsOccupied)
                    return false;
            return true;
        }
    }

    void Start()
    {
        _holders = _itemHoldersParent.GetComponentsInChildren<ItemHolder>();
        _itemHolderCount = _holders.Length;
        if (_itemHolderCount <= 0)
        {
            Debug.LogError("Error. No item holders found under item holders parent of counter " + name);
        }
    }

    public void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetComponent<PlayerHand>();

        if (hand.IsHoldingItem)
        {
            if (CounterIsFull)
            {
                Debug.Log("Counter is full");
            }
            else
            {
                PlaceItemFromHand(hand);
            }
        }
        else 
        {
            if (CounterIsEmpty)
            {
                TakeFromEmptyCounter(hand);
            }
            else
            {
                TakeItemFromCounter(hand);
            }
        }
    }

    void PlaceItemFromHand(PlayerHand hand)
    {
        Item item = hand.GetItem();
        hand.ReleaseItem();
        _holders[GetNextFreeHolderIndex()].SetItem(item);
    }

    protected virtual void TakeFromEmptyCounter(PlayerHand hand)
    {
        Debug.Log("Nothing to take");
    }

    void TakeItemFromCounter(PlayerHand hand)
    {
        ItemHolder holder = _holders[CounterIsFull ? _holders.Length - 1 : GetNextFreeHolderIndex() - 1];
        Item item = holder.GetItem();
        holder.SetItem(null);
        hand.SetItem(item);
    }

    int GetNextFreeHolderIndex()
    {
        if (CounterIsFull)
            return -1;

        for (int i = 0; i < _holders.Length; i++)
            if (!_holders[i].IsOccupied)
                return i;

        return -1;
    }
}
