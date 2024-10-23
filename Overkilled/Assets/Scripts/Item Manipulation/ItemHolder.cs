using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ItemHolder : NetworkBehaviour
{
    [Tooltip("The position to hold items at")]
    [SerializeField] Transform _holdPosition;
    [Tooltip("The multiplying factor to apply to objects when being held")]
    [SerializeField] float _itemShrinkfactor = 1f;
    [Tooltip("The rotation to set to objects when being held")]
    [SerializeField] Vector3 _lockRotation = Vector3.zero;

    /// <summary>
    /// Whether this holder is currently holding an object
    /// </summary>
    public bool IsOccupied { get; private set; } = false;

    NetworkObject _networkObject;
    Item _currentItem;

    void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        _currentItem = null;
        IsOccupied = false;
    }

    /// <summary>
    /// Get the current item thats being held
    /// </summary>
    /// <returns></returns>
    public Item GetItem() { return _currentItem; }

    public NetworkObject GetNetworkObject() { return _networkObject; } 

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
        SetItemServerRpc(item ? item.GetNetworkObject() : null);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetItemServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        if (itemNetworkObject != null && !IsOccupied)
        {
            itemNetworkObject.TrySetParent(_networkObject);
            SetItemClientRpc(itemNetworkObjectReference);
        }
        else if (itemNetworkObject == null && IsOccupied)
        {
            _currentItem.GetNetworkObject().TryRemoveParent();
            SetItemClientRpc(new NetworkObjectReference());
        }
    }

    [ClientRpc]
    void SetItemClientRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);

        if (itemNetworkObject != null)
        {
            _currentItem = itemNetworkObject.GetComponent<Item>();
            IsOccupied = true;
            SetLockItem(true, true);
        }
        else
        {
            SetLockItem(false, true);
            _currentItem = null;
            IsOccupied = false;
        }
    }

    public void SyncHolder(Item item)
    {
        if (item != null)
        {
            _currentItem = item;
            IsOccupied = true;
            SetLockItem(true, false);
        }
        else 
        {
            SetLockItem(false, false);
            _currentItem = null;
            IsOccupied = false;
        }
    }

    void SetLockItem(bool lockState, bool useShrink)
    {
        _currentItem.ToggleItemLock(lockState);

        if (lockState == true)
        {
            _currentItem.transform.position = _holdPosition.transform.position;
            _currentItem.transform.localRotation = Quaternion.Euler(_lockRotation);
            
            if (useShrink)
                _currentItem.transform.localScale *= _itemShrinkfactor;
        }
        else
        {
            if (useShrink)
                _currentItem.transform.localScale /= _itemShrinkfactor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_holdPosition.position, Vector3.one / 8f);
    }
}