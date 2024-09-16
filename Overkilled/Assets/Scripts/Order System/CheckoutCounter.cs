using Unity.Netcode;
using UnityEngine;

public class CheckoutCounter : CounterTop
{
    public override void Interact(PlayerInteraction player)
    {
        if (!player.IsHoldingItem)
            return;

        PlayerHand hand = player.GetHand();

        //Check if active order for item held
        Item item = hand.GetItem();
        if (!OrderSystem.Instance.CheckActiveOrder(item.GetItemInfo())) 
            return;

        //Money based on weapon durability
        float durabilityFactor = 1f;
        Weapon weapon = hand.GetItem().GetComponent<Weapon>();
        if (weapon != null)
            durabilityFactor = (float)weapon.Durability / weapon.GetWeaponInfo().durability;
        OrderSystem.Instance.DeliverRecipe(item.GetItemInfo(), durabilityFactor);

        //Placing logic
        base.Interact(player);

        ////Pack and send away? To update
        MultiplayerManager.Instance.DestroyObject(item.gameObject, 1f);
        _holders[0].SetItem(null);
    }
}
