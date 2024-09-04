using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour, IInteractable
{
    [Tooltip("The item scriptable object for this item")]
    [SerializeField] ItemSO _itemSO;

    void Awake()
    {
        if (_itemSO == null)
            Debug.LogWarning("Warning. Item " + name + "'s ScriptableObject is not assign");
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

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
}
