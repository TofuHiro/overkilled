using SurvivalGame;
using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public const int MAX_PLAYER_COUNT = 4;
    const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    [SerializeField] NetworkPrefabsList _networkPrefabsList;

    public static MultiplayerManager Instance { get; private set; }

    public static event Action OnLocalDisconnect;
    public static event Action OnTryingToJoinGame;
    public static event Action OnFailedToJoinGame;
    public static event Action OnPlayerDataNetworkListChange;

    NetworkList<PlayerData> _playerDataNetworkList;
    NetworkObject _previousSpawnedObject;
    string _playerName;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(0, 1000).ToString());
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public string GetPlayerName()
    {
        return _playerName;
    }

    public void SetPlayerName(string newName)
    {
        _playerName = newName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, _playerName);////
    }

    void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChange?.Invoke();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += SetConnectionApproval;
        NetworkManager.Singleton.OnConnectionEvent += AddPlayerToNetworkList;
        NetworkManager.Singleton.OnConnectionEvent += Server_OnClientDisconnect;
        NetworkManager.Singleton.StartHost();
    }

    void SetConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }
        
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT)
        {
            response.Approved = false;
            response.Reason = "Lobby is full";
            return;
        }

        response.Approved = true;
    }

    void AddPlayerToNetworkList(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            _playerDataNetworkList.Add(new PlayerData
            {
                clientId = data.ClientId
            });

            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            SetPlayerNameServerRpc(GetPlayerName());
        }
    }

    void Server_OnClientDisconnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected)
            RemovePlayerData(data.ClientId);
    }

    void RemovePlayerData(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = _playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                _playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke();

        NetworkManager.Singleton.OnConnectionEvent += Client_FailToJoin;
        NetworkManager.Singleton.OnConnectionEvent += Client_OnLocalDisconnectEvent;
        NetworkManager.Singleton.OnConnectionEvent += Client_OnConnectEvent;
        NetworkManager.Singleton.StartClient();
    }

    void Client_FailToJoin(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            OnFailedToJoinGame?.Invoke();
        }
    }

    void Client_OnLocalDisconnectEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            OnLocalDisconnect?.Invoke();
        }
    }

    void Client_OnConnectEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            SetPlayerNameServerRpc(GetPlayerName());
        }
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        RemovePlayerData(clientId);
    }

    public bool IsPlayerIndexConnected(int index)
    {
        return index < _playerDataNetworkList.Count;
    }

    #region Player Data

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
            if (_playerDataNetworkList[i].clientId == clientId)
                return i;

        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
            if (playerData.clientId == clientId)
                return playerData;

        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int index)
    {
        return _playerDataNetworkList[index];
    }

    public void CyclePlayerModel(bool forward)
    {
        CyclePlayerModelServerRpc(forward);
    }

    [ServerRpc(RequireOwnership = false)]
    void CyclePlayerModelServerRpc(bool forward, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        if (forward)
            playerData.PlayerModelId++;
        else
            playerData.PlayerModelId--;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

       _playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    #endregion

    #region Item Spawning

    int GetIndexFromObject(GameObject obj)
    {
        for (int i = 0; i < _networkPrefabsList.PrefabList.Count; i++)
            if (_networkPrefabsList.PrefabList[i].Prefab.name == obj.name)
                return i;

        return -1;
    }

    GameObject GetObjectFromIndex(int index)
    {
        return _networkPrefabsList.PrefabList[index].Prefab;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The network object of the spawned item</returns>
    public NetworkObject SpawnItem(GameObject item)
    {
        SpawnItemServerRpc(GetIndexFromObject(item));
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position">The position to spawn at</param>
    /// <param name="rotation">The rotation to spawn with</param>
    /// <returns></returns>
    public NetworkObject SpawnItem(GameObject item, Vector3 position, Quaternion rotation)
    {
        SpawnItem(item);
        _previousSpawnedObject.transform.position = position;
        _previousSpawnedObject.transform.rotation = rotation;
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="hand">The hand to set the items parents to</param>
    /// <returns></returns>
    public NetworkObject SpawnItem(GameObject item, PlayerHand hand)
    {
        SpawnItemWithParentServerRpc(GetIndexFromObject(item), hand.GetNetworkObject());
        return _previousSpawnedObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemServerRpc(int objectIndex)
    {
        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        _previousSpawnedObject = networkObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemWithParentServerRpc(int objectIndex, NetworkObjectReference handNetworkObjectReference)
    {
        handNetworkObjectReference.TryGet(out NetworkObject handNetworkObject);

        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        PlayerHand hand = handNetworkObject.GetComponent<PlayerHand>();
        hand.SetItem(networkObject.GetComponent<Item>());

        _previousSpawnedObject = networkObject;
    }

    public void DestroyItem(GameObject item)
    {
        DestroyItemServerRpc(item.GetComponent<NetworkObject>(), 0);
    }

    public void DestroyItem(GameObject item, float delay)
    {
        DestroyItemServerRpc(item.GetComponent<NetworkObject>(), delay);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyItemServerRpc(NetworkObjectReference itemNetworkObjectReference, float delay)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Destroy(itemNetworkObject.gameObject, delay);
    }

    #endregion

    
}
