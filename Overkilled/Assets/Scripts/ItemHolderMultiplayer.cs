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
        //Event resub to avoid infinite loop
        _itemHolder.OnItemChange -= ItemHolder_OnItemChange;
        SetItemServerRpc(item ? item.GetNetworkObject() : null);
        _itemHolder.OnItemChange += ItemHolder_OnItemChange;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetItemServerRpc(NetworkObjectReference itemNetworkObjectReference, ServerRpcParams serverRpcParams = default)
    {
        //As local client already set from ItemHolder
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.LocalClientId)
        {
            SetItemClientRpc(itemNetworkObjectReference);
        }
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
