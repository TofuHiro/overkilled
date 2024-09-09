using Unity.Netcode;
using UnityEngine;

public class CheckoutCounter : CounterTop
{
    OrderSystem _orderSystem;
    MultiplayerManager _multiplayerManager;

    protected override void Start()
    {
        base.Start();
        _orderSystem = OrderSystem.Instance;
        _multiplayerManager = MultiplayerManager.Instance;
    }

    public override void Interact(PlayerInteraction player)
    {
        PlayerHand hand = player.GetComponent<PlayerHand>();
        if (!hand.IsHoldingItem)
            return;

        Item item = hand.GetItem();
        if (!_orderSystem.CheckActiveOrder(item.GetItemInfo())) 
            return;

        //Money based on weapon durability
        Weapon weapon = hand.GetItem().GetComponent<Weapon>();
        float durabilityFactor = 1f;

        if (weapon != null)
            durabilityFactor = (float)weapon.Durability / weapon.GetWeaponInfo().durability;
        _orderSystem.DeliverRecipe(item.GetItemInfo(), durabilityFactor);

        //Placing logic
        base.Interact(player);

        //Pack and send away? To update
        _multiplayerManager.DestroyItem(item.gameObject, 1f);
        _holders[0].SetItem(null);
    }
}
