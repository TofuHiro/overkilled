using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyLateJoinManager : NetworkBehaviour
{
    public static LobbyLateJoinManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyLateJoinManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            PingServerSyncServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PingServerSyncServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        };

        SyncPlayerClientRpc(clientRpcParams);
    }

    [ClientRpc]
    void SyncPlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {

    }

    void SyncSelectedLevel()
    {

    }
}
