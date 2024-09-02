using Unity.Netcode;

public class CheckoutCounter : CounterTop
{
    OrderSystem _orderSystem;

    protected override void Start()
    {
        base.Start();
        _orderSystem = OrderSystem.Instance;
    }

    public override void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetComponent<PlayerHand>();
        if (!hand.IsHoldingItem)
            return;

        ItemSO item = hand.GetItem().GetItemInfo();
        if (!_orderSystem.CheckActiveOrder(item)) 
            return;

        //Money based on weapon durability
        Weapon weapon = hand.GetItem().GetComponent<Weapon>();
        float durabilityFactor = 1f;

        if (weapon != null)
            durabilityFactor = (float)weapon.Durability / weapon.GetWeaponInfo().durability;
        _orderSystem.DeliverRecipe(item, durabilityFactor);

        //Placing logic
        base.Interact(player);

        //Pack and send away? To update
        DestroyItemServerRpc(_holders[0].GetItem().GetNetworkObject());

        ReleaseAllItems();
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyItemServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Destroy(itemNetworkObject.gameObject, 2f);
    }
}
