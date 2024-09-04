using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class PlayerHand : NetworkBehaviour
{
    [Tooltip("Force applied to objects when dropping them")]
    [SerializeField] float _dropThrowForce = 5f;
    public bool IsHoldingItem { get { return _holder.IsOccupied; } }

    ItemHolder _holder;
    Weapon _currentWeapon;

    void Awake()
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

    /// <summary>
    /// Assigns a new item to the player's hand
    /// </summary>
    /// <param name="newItem"></param>
    public void SetItem(Item newItem)
    {
        if (IsHoldingItem)
            return;

        _holder.SetItem(newItem);

        _currentWeapon = newItem.GetComponent<Weapon>();
        _currentWeapon?.OnPickup();
    }

    /// <summary>
    /// Drop item the player is currently holding and apply a throwing force
    /// </summary>
    public void DropItem()
    {
        Item item = _holder.GetItem();
        ReleaseItem();
        item.GetComponent<Rigidbody>().AddForce(transform.forward * _dropThrowForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Releases item from the players hand. Player will no longer hold anything
    /// </summary>
    public void ReleaseItem()
    {
        if (_holder.GetItem() == null)
            return;

        _holder.SetItem(null);

        if (_currentWeapon != null)
        {
            _currentWeapon.OnDrop();
            _currentWeapon = null;
        }
    }

    /// <summary>
    /// Returns the network object of this player
    /// </summary>
    /// <returns></returns>
    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
}
