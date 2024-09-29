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
    [Tooltip("If is static, when parenting, object will just be placed at hold position once, while if not static, the object will constantly be set to the position")]
    [SerializeField] bool _isStatic = true;

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

    void Update()
    {
        if (_isStatic)
            return;

        if (IsOccupied)
        {
            _currentItem.transform.position = _holdPosition.position;
            _currentItem.transform.rotation = _holdPosition.rotation;
        }
    }


    /// <summary>
    /// Assigns a new item to this holder, locking it into place
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        SetItemServerRpc(item ? item.GetNetworkObject() : null, true);
    }

    /// <summary>
    /// Specify whether to shrink the item using this item holders shrink scale settings or not
    /// </summary>
    /// <param name="item"></param>
    /// <param name="useShrink"></param>
    public void SetItem(Item item, bool useShrink)
    {
        SetItemServerRpc(item ? item.GetNetworkObject() : null, useShrink);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetItemServerRpc(NetworkObjectReference itemNetworkObjectReference, bool useShrink)
    {
        SetItemClientRpc(itemNetworkObjectReference, useShrink);
    }

    [ClientRpc]
    void SetItemClientRpc(NetworkObjectReference itemNetworkObjectReference, bool useShrink)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);

        if (itemNetworkObject != null)
        {
            Item item = itemNetworkObject.GetComponent<Item>();
            _currentItem = item;
            IsOccupied = true;
            SetLockItem(true, useShrink);
        }
        else
        {
            SetLockItem(false, useShrink);
            _currentItem = null;
            IsOccupied = false;
        }
    }

    public void SetLockItem(bool lockState, bool useShrink)
    {
        _currentItem.ToggleItemLock(lockState);

        if (lockState == true)
        {
            _currentItem.transform.position = _holdPosition.transform.position;
            _currentItem.transform.rotation = Quaternion.Euler(_lockRotation);
            
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