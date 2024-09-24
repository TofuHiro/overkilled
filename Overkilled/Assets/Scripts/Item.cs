using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour, IInteractable
{
    [Tooltip("The item scriptable object for this item")]
    [SerializeField] ItemSO _itemSO;

    Rigidbody _rigidbody;
    Collider[] _colliders;

    void Awake()
    {
        if (_itemSO == null)
            Debug.LogWarning("Warning. Item " + name + "'s ScriptableObject is not assign");

        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>();
    }

    public ItemSO GetItemInfo() { return _itemSO; }

    public void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetComponent<PlayerHand>();
        if (hand != null)
        {
            hand.SetItem(this);
        }
    }

    public void ToggleItemLock(bool state)
    {
        _rigidbody.isKinematic = state;

        foreach (Collider collider in _colliders)
            collider.enabled = !state;
    }

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
}
