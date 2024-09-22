using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerHand))]
public class PlayerHandMultiplayer : NetworkBehaviour
{
    PlayerHand _hand;

    void Awake()
    {
        _hand = GetComponent<PlayerHand>();    
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += DropItemOnDisconnect;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= DropItemOnDisconnect;
    }

    private void DropItemOnDisconnect(NetworkManager manager, ConnectionEventData data)
    {
        if (!_hand.IsHoldingItem)
            return;

        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == OwnerClientId)
            _hand.DropItem();
    }
}
