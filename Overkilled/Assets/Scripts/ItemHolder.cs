using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Transform _holdPosition;

    public bool IsOccupied { get; private set; } = false;

    public Item CurrentItem
    {
        get { return _currentItem; }
        set
        {
            _currentItem = value;

            if (_currentItem != null)
            {
                _currentRigidbody = _currentItem.GetComponent<Rigidbody>();
                _currentColliders = _currentItem.GetComponents<Collider>();
                IsOccupied = true;
            }
            else
            {
                _currentRigidbody = null;
                _currentColliders = null;
                IsOccupied = false;
            }
        }
    }
    private Item _currentItem;

    Rigidbody _currentRigidbody;
    Collider[] _currentColliders;

    public void SetLockItem(bool state)
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