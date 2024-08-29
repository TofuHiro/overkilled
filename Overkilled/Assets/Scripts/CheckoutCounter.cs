using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (!_orderSystem.CheckForOrder(item)) 
            return;

        //Money
        Weapon weapon = hand.GetItem().GetComponent<Weapon>();
        float durabilityFactor = 1f;

        if (weapon != null)
            durabilityFactor = (float)weapon.Durability / weapon.GetWeaponInfo().durability;
        _orderSystem.CompleteOrder(item, durabilityFactor);

        //Placing logic
        base.Interact(player);

        //Pack and send away?
        Destroy(_holders[0].GetItem().gameObject, 2f);//
        ReleaseAllItems();
    }
}
