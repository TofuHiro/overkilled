using Unity.Netcode;
using UnityEngine;

public class ItemHolder : NetworkBehaviour
{
    [Tooltip("The position to hold items at")]
    [SerializeField] Transform _holdPosition;
    [Tooltip("The multiplying factor to apply to objects when being held")]
    [SerializeField] float _itemShrinkfactor = 1f;

    /// <summary>
    /// Whether this holder is currently holding an object
    /// </summary>
    public bool IsOccupied { get; private set; } = false;

    Item _currentItem;

    public override void OnNetworkSpawn()
    {
        if (_currentItem == null)
        {
            _currentItem = null;
            IsOccupied = false;
        }
    }

    /// <summary>
    /// Get the current item thats being held
    /// </summary>
    /// <returns></returns>
    public Item GetItem() { return _currentItem; }

    /// <summary>
    /// The position of where the item is held at
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHoldPosition() { return _holdPosition.position; }

    /// <summary>
    /// Assigns a new item to this holder, locking it into place
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        if (item != null)
        {
            SetItemServerRpc(item.GetNetworkObject());
        }
        else
        {
            RemoveItemServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    void SetItemServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        // If already set
        if (_currentItem != null)
            return;

        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);

        itemNetworkObject.TrySetParent(gameObject);
        SetItemClientRpc(itemNetworkObjectReference);

    }
    [Rpc(SendTo.Everyone)]
    void SetItemClientRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Item item = itemNetworkObject.GetComponent<Item>();

        _currentItem = item;
        IsOccupied = true;
        LockItem(true, true);

        _currentItem.transform.SetPositionAndRotation(_holdPosition.position, _holdPosition.rotation);
    }

    [Rpc(SendTo.Server)]
    void RemoveItemServerRpc()
    {
        // If already taken
        if (_currentItem == null)
            return;

        NetworkObject networkObject = _currentItem.GetNetworkObject();

        RemoveItemClientRpc();

        networkObject.TryRemoveParent(true);
    }
    [Rpc(SendTo.Everyone)]
    void RemoveItemClientRpc()
    {
        LockItem(false, true);
        _currentItem = null;
        IsOccupied = false;
    }

    public void SyncHolder(Item item)
    {
        if (item != null)
        {
            _currentItem = item;
            IsOccupied = true;
            LockItem(true, false);
        }
        else 
        {
            LockItem(false, false);
            _currentItem = null;
            IsOccupied = false;
        }
    }

    void LockItem(bool lockState, bool useShrink)
    {
        _currentItem.ToggleItemLock(lockState);

        if (useShrink)
        {
            if (lockState)
            {
                _currentItem.transform.localScale *= _itemShrinkfactor;
            }
            else
            {
                _currentItem.transform.localScale /= _itemShrinkfactor;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_holdPosition.position, Vector3.one / 8f);
    }
}