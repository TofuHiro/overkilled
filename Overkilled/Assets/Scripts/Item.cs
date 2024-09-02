using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemSO _itemInfo;

    public ItemSO GetItemInfo() { return _itemInfo; }

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
