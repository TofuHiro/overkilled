using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class PlayerHand : NetworkBehaviour
{
    [Tooltip("Force applied to objects when dropping them")]
    [SerializeField] float _dropThrowForce = 5f;
    [Tooltip("Force applied to objects when throwing them")]
    [SerializeField] float _throwForce = 20f;

    public event Action OnWeaponPickUp;
    public event Action OnWeaponDrop;

    public delegate void AttackAction(float movementSpeedMultiplier);
    public event AttackAction OnSecondaryAttackStart;
    public event AttackAction OnSecondaryAttackStop;

    /// <summary>
    /// Whether this hand is holding an item
    /// </summary>
    public bool IsHoldingItem { get { return _holder.IsOccupied; } }

    ItemHolder _holder;
    Weapon _currentWeapon;

    void Awake()
    {
        _holder = GetComponent<ItemHolder>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += DropItemOnDisconnect;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= DropItemOnDisconnect;
    }

    void DropItemOnDisconnect(NetworkManager manager, ConnectionEventData data)
    {
        if (!IsHoldingItem)
            return;

        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == OwnerClientId)
            DropItem();
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

        float movementSpeedMultiplier = _currentWeapon.GetWeaponInfo().secondaryAttackMovementSpeedMultiplier;
        if (state)
            OnSecondaryAttackStart?.Invoke(movementSpeedMultiplier);
        else
            OnSecondaryAttackStop?.Invoke(movementSpeedMultiplier);
    }

    /// <summary>
    /// Get the item this holder is currently assigned
    /// </summary>
    /// <returns></returns>
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
        if (_currentWeapon != null)
        {
            SetAttackState(false);
            SetSecondaryAttackState(false);

            OnWeaponPickUp?.Invoke();
        }
    }

    /// <summary>
    /// Drop item the player is currently holding and apply a throwing force
    /// </summary>
    public void DropItem()
    {
        Item item = _holder.GetItem();
        ThrowItemServerRpc(item.GetNetworkObject(), _dropThrowForce);
    }

    public void ThrowItem()
    {
        Item item = _holder.GetItem();
        ThrowItemServerRpc(item.GetNetworkObject(), _throwForce);
    }

    [ServerRpc(RequireOwnership = false)]
    void ThrowItemServerRpc(NetworkObjectReference itemObjectReference, float force)
    {
        if (_holder.GetItem() != null)
        {
            ReleaseItem();
            ThrowItemClientRpc(itemObjectReference, force);
        }
    }

    [ClientRpc]
    void ThrowItemClientRpc(NetworkObjectReference itemObjectReference, float force)
    {
        itemObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Item item = itemNetworkObject.GetComponent<Item>();
        
        item.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
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
            SetAttackState(false);
            SetSecondaryAttackState(false);
            _currentWeapon = null;

            OnWeaponDrop?.Invoke();
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
