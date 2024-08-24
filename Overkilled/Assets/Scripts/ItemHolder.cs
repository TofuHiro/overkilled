using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Transform _holdPosition;
    [SerializeField] float _itemShrinkfactor = 1f;
    [SerializeField] Vector3 _lockRotation = Vector3.zero;

    public bool IsOccupied { get; private set; } = false;

    Item _currentItem;
    Rigidbody _currentRigidbody;
    Collider[] _currentColliders;

    public Item GetItem() { return _currentItem; }
    public Vector3 GetHoldPosition() { return _holdPosition.position; }

    public void SetItem(Item item)
    {
        if (item != null)
        {
            _currentItem = item;
            _currentRigidbody = item.GetComponent<Rigidbody>();
            _currentColliders = item.GetComponents<Collider>();
            IsOccupied = true;
            SetLockItem(true);
        }
        else
        {
            SetLockItem(false);
            _currentItem = null;
            _currentRigidbody = null;
            _currentColliders = null;
            IsOccupied = false;
        }
    }

    void SetLockItem(bool state)
    {
        _currentRigidbody.isKinematic = state;
        _currentItem.transform.SetParent(state? _holdPosition : null);

        foreach (Collider collider in _currentColliders)
            collider.enabled = !state;
        if (state == true)
        {
            _currentItem.transform.position = _holdPosition.transform.position;
            _currentItem.transform.localRotation = Quaternion.Euler(_lockRotation);
            _currentItem.transform.localScale *= _itemShrinkfactor;
        }
        else
        {
            _currentItem.transform.localScale /= _itemShrinkfactor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_holdPosition.position, Vector3.one / 8f);
    }
}