using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Transform _holdPosition;

    public bool IsHoldingItem { get; private set; } = false;

    Item _currentItem;
    Rigidbody _currentRigidbody;
    Collider[] _currentColliders;

    public void SetItem(Item item)
    {
        if (item != null)
        {
            _currentItem = item;
            _currentRigidbody = item.GetComponent<Rigidbody>();
            _currentColliders = item.GetComponents<Collider>();
            IsHoldingItem = true;
            SetLockItem(true);
        }
        else
        {
            _currentItem = null;
            _currentRigidbody = null;
            _currentColliders = null;
            IsHoldingItem = false;
        }
    }

    public void DropItem()
    {
        if (_currentItem == null)
            return;

        SetLockItem(false);

        //Force
        SetItem(null);
    }

    public void ThrowItem()
    {
        if (_currentItem == null)
            return;

        SetLockItem(false);

        //Force
        SetItem(null);
    }

    void SetLockItem(bool state)
    {
        foreach (Collider collider in _currentColliders)
            collider.enabled = !state;
        if (state == true)
            _currentItem.transform.position = _holdPosition.transform.position;

        _currentRigidbody.isKinematic = state;
        _currentItem.transform.SetParent(state? _holdPosition : null);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_holdPosition.position, Vector3.one / 8f);    
    }
}