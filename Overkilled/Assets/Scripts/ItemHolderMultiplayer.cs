using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ItemHolder))]
public class ItemHolderMultiplayer : NetworkBehaviour
{
    ItemHolder _itemHolder;

    void Awake()
    {
        _itemHolder = GetComponent<ItemHolder>();

        _itemHolder.OnItemChange += ItemHolder_OnItemChange;
    }

    void ItemHolder_OnItemChange(Item item)
    {
        SetItemServerRpc(item ? item.GetNetworkObject() : null);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetItemServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        SetItemClientRpc(itemNetworkObjectReference);
    }

    [ClientRpc]
    void SetItemClientRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);

        if (itemNetworkObject != null)
        {
            Item item = itemNetworkObject.GetComponent<Item>();
            _itemHolder.SetItem(item);
        }
        else
        {
            _itemHolder.SetItem(null);
        }
    }
}
