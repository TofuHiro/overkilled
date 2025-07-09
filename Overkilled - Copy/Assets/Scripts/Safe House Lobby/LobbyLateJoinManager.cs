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

    List<ItemHolder> _itemHoldersToSync = new List<ItemHolder>();
    List<ItemHolder> _playerItemHoldersToSync = new List<ItemHolder>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyLateJoinManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        PlayerList.OnPlayerAdd += PlayerList_OnPlayerAdd;
        PlayerList.OnPlayerRemove += PlayerList_OnPlayerRemove;
    }

    public override void OnDestroy()
    {
        PlayerList.OnPlayerAdd -= PlayerList_OnPlayerAdd;
        PlayerList.OnPlayerRemove -= PlayerList_OnPlayerRemove;
    }

    void Start()
    {
        PopulateItemHolderToSyncList();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            PingServerSyncServerRpc();
        }
    }

    void PlayerList_OnPlayerAdd(GameObject player)
    {
        ItemHolder holder = player.GetComponentInChildren<ItemHolder>();
        _playerItemHoldersToSync.Add(holder);
    }

    void PlayerList_OnPlayerRemove(GameObject player)
    {
        ItemHolder holder = player.GetComponentInChildren<ItemHolder>();
        _playerItemHoldersToSync.Remove(holder);
    }

    void PopulateItemHolderToSyncList()
    {
        ItemHolder[] holders = _countersParent.GetComponentsInChildren<ItemHolder>();
        foreach (ItemHolder holder in holders)
            _itemHoldersToSync.Add(holder);
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
        SyncSelectedLevelClientRpc(LevelSelectManager.Instance.CurrentLevel, clientRpcParams);



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
    void SyncSelectedLevelClientRpc(Level selectedLevel, ClientRpcParams clientRpcParams = default)
    {
        LevelSelectManager.Instance.SetLevel(selectedLevel);
    }

    [ClientRpc]
    void SyncItemHoldersClientRpc(NetworkObjectReference[] networkObjectReferences, ClientRpcParams clientRpcParams = default)
    {
        for (int i = 0; i < _itemHoldersToSync.Count; i++)
        {
            networkObjectReferences[i].TryGet(out NetworkObject itemNetworkObject);
            if (itemNetworkObject != null)
                _itemHoldersToSync[i].SyncHolder(itemNetworkObject.GetComponent<Item>());
        }
    }

    [ClientRpc]
    void SyncPlayerItemHoldersClientRpc(NetworkObjectReference[] networkObjectReferences, ClientRpcParams clientRpcParams = default)
    {
        for (int i = 0; i < _playerItemHoldersToSync.Count; i++)
        {
            networkObjectReferences[i].TryGet(out NetworkObject itemNetworkObject);
            if (itemNetworkObject != null)
                _playerItemHoldersToSync[i].SyncHolder(itemNetworkObject.GetComponent<Item>());
        }
    }
}
