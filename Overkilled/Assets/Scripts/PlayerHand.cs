using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class PlayerHand : MonoBehaviour
{
    [SerializeField] float _dropThrowForce = 5f;
    public bool IsHoldingItem { get { return _holder.IsOccupied; } }

    ItemHolder _holder;

    void Start()
    {
        _holder = GetComponent<ItemHolder>();
    }

    public Item GetItem()
    {
        return _holder.GetItem();
    }

    public void SetItem(Item newItem)
    {
        _holder.SetItem(newItem);
    }

    public void DropItem()
    {
        Item item = _holder.GetItem();

        ReleaseItem();

        item.GetComponent<Rigidbody>().AddForce(transform.forward * _dropThrowForce, ForceMode.Impulse);
    }

    public void ReleaseItem()
    {
        if (_holder.GetItem() == null)
            return;

        _holder.SetItem(null);
    }
}
