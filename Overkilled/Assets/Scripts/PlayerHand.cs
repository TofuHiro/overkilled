using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class PlayerHand : MonoBehaviour
{
    [SerializeField] float _dropThrowForce = 5f;
    public bool IsHoldingItem { get { return _holder.IsOccupied; } }

    ItemHolder _holder;
    Weapon _currentWeapon;

    void Start()
    {
        _holder = GetComponent<ItemHolder>();
    }

    public void SetAttackState(bool state)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.SetAttackState(state);
    }

    public void SetSecondaryAttackState(bool state)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.SetSecondaryAttackState(state);
    }

    public Item GetItem()
    {
        return _holder.GetItem();
    }

    public void SetItem(Item newItem)
    {
        _holder.SetItem(newItem);

        _currentWeapon = newItem.GetComponent<Weapon>();
        if (_currentWeapon != null )
            _currentWeapon.OnPickup();
    }

    public void DropItem()
    {
        Item item = _holder.GetItem();
        ReleaseItem();
        item.GetComponent<Rigidbody>().AddForce(transform.forward * _dropThrowForce, ForceMode.Impulse);

        if (_currentWeapon != null)
        {
            _currentWeapon.OnDrop();
            _currentWeapon = null;
        }
    }

    public void ReleaseItem()
    {
        if (_holder.GetItem() == null)
            return;

        _holder.SetItem(null);
    }
}
