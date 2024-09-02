using Unity.Netcode;
using UnityEngine;

public class MaterialBox : CounterTop
{
    [Tooltip("The source object to spawn from this box")]
    [SerializeField] GameObject _materialPrefab;

    protected override void TakeFromEmptyCounter(PlayerHand hand)
    {
        SpawnItemServerRpc(hand.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemServerRpc(NetworkObjectReference handNetworkObjectReference)
    {
        GameObject materialTranform = Instantiate(_materialPrefab);
        materialTranform.GetComponent<NetworkObject>().Spawn(true);

        handNetworkObjectReference.TryGet(out NetworkObject handNetworkObject);

        PlayerHand hand = handNetworkObject.GetComponent<PlayerHand>();
        hand.SetItem(materialTranform.GetComponent<Item>());
    }
}
