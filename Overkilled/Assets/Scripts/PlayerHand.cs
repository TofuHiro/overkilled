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

    public void DropItem()
    {
        if (_holder.CurrentItem == null)
            return;

        _holder.SetLockItem(false);
        _holder.CurrentItem.GetComponent<Rigidbody>().AddForce(transform.forward * _dropThrowForce, ForceMode.Impulse);
        _holder.CurrentItem = null;
    }
}
