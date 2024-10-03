using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyLateJoinManager : NetworkBehaviour
{
    [Header("Item Holder Syncing")]
    [SerializeField] Transform _countersParent;

    public static LobbyLateJoinManager Instance { get; private set; }

    List<ItemHolder> _itemHoldersToSync;
    List<ItemHolder> _playerItemHoldersToSync;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyLateJoinManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _itemHoldersToSync = new List<ItemHolder>();
        _playerItemHoldersToSync = new List<ItemHolder>();
    }

    void Start()
    {
        PlayerList.OnPlayerListUpdate += AddNewPlayerItemHolder;

        PopulateItemHolderToSyncList();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            PingServerSyncServerRpc();
        }
    }

    void PopulateItemHolderToSyncList()
    {
        ItemHolder[] holders = _countersParent.GetComponentsInChildren<ItemHolder>();
        foreach (ItemHolder holder in holders)
            _itemHoldersToSync.Add(holder);
    }

    void AddNewPlayerItemHolder()
    {
        _playerItemHoldersToSync = new List<ItemHolder>();

        GameObject[] players = PlayerList.GetPlayers();
        foreach (GameObject player in players)
        {
            ItemHolder holder = player.GetComponentInChildren<ItemHolder>();
            _playerItemHoldersToSync.Add(holder);
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



        //Level Selection
        SyncSelectedLevelClientRpc(LobbyManager.Instance.GetSelectedLevel(), clientRpcParams);



        //Item Holders
        NetworkObjectReference[] itemHolderNetworkObjects = new NetworkObjectReference[_itemHoldersToSync.Count];
        for (int i = 0; i < _itemHoldersToSync.Count; i++)
        {
            Item item = _itemHoldersToSync[i].GetItem();
            itemHolderNetworkObjects[i] = item ? item.GetNetworkObject() : null;
        }
        SyncItemHoldersClientRpc(itemHolderNetworkObjects, clientRpcParams);

        NetworkObjectReference[] playerItemHolderNetworkObjects = new NetworkObjectReference[_playerItemHoldersToSync.Count];
        for (int i = 0; i < _playerItemHoldersToSync.Count; i++)
        {
            Item item = _playerItemHoldersToSync[i].GetItem();
            playerItemHolderNetworkObjects[i] = item ? item.GetNetworkObject() : null;
        }

        SyncPlayerItemHoldersClientRpc(playerItemHolderNetworkObjects, clientRpcParams);
    }

    [ClientRpc]
    void SyncSelectedLevelClientRpc(Loader.Level selectedLevel, ClientRpcParams clientRpcParams = default)
    {
        LobbyManager.Instance.SetLevel(selectedLevel);
    }

    [ClientRpc]
    void SyncItemHoldersClientRpc(NetworkObjectReference[] networkObjectReferences, ClientRpcParams clientRpcParams = default)
    {
        for (int i = 0; i < _itemHoldersToSync.Count; i++)
        {
            networkObjectReferences[i].TryGet(out NetworkObject itemNetworkObject);
            if (itemNetworkObject != null)
                _itemHoldersToSync[i].SetItem(itemNetworkObject.GetComponent<Item>(), false);
        }
    }

    [ClientRpc]
    void SyncPlayerItemHoldersClientRpc(NetworkObjectReference[] networkObjectReferences, ClientRpcParams clientRpcParams = default)
    {
        for (int i = 0; i < _playerItemHoldersToSync.Count; i++)
        {
            networkObjectReferences[i].TryGet(out NetworkObject itemNetworkObject);
            if (itemNetworkObject != null)
                _playerItemHoldersToSync[i].SetItem(itemNetworkObject.GetComponent<Item>(), false);
        }
    }
}
