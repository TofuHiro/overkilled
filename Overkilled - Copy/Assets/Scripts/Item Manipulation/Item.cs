using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour, IInteractable
{
    [Tooltip("The item scriptable object for this item")]
    [SerializeField] ItemSO _itemSO;

    Rigidbody _rigidbody;
    Collider[] _colliders;
    NetworkObject _networkObject;
    PredictedPhysicsObject _physicsObject;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>();
        _networkObject = GetComponent<NetworkObject>();
        _physicsObject = GetComponent<PredictedPhysicsObject>();

        if (_itemSO == null)
            Debug.LogWarning("Warning. Item " + name + "'s ScriptableObject is not assigned");
    }

    public ItemSO GetItemInfo() { return _itemSO; }

    public void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetHand();
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

        if (_physicsObject)
            _physicsObject.ToggleSync(!state);
    }

    public NetworkObject GetNetworkObject()
    {
        return _networkObject;
    }
}
