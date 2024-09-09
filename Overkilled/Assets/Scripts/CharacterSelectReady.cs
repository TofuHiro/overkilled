using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance;

    Dictionary<ulong, bool> _playerReadyDictionary;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _playerReadyDictionary = new Dictionary<ulong, bool>();    
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    /// <summary>
    /// Checks if all players in a server are ready and loads the game scene
    /// </summary>
    /// <param name="serverRpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayersReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            Loader.LoadSceneNetwork(Loader.Scene.GameScene);
        }
    }
}
