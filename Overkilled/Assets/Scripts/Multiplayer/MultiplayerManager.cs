using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public const int MAX_PLAYER_COUNT = 4;
    const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    [Tooltip("Network prefab list used for spawning objects. It is used to find the index of an item by reference and spawned from this list")]
    [SerializeField] NetworkPrefabsList _networkPrefabsList;

    public static MultiplayerManager Instance { get; private set; }

    public event Action OnLocalDisconnect;
    public event Action OnPlayerDataNetworkListChange;

    NetworkList<PlayerData> _playerDataNetworkList;
    //Used to return spawned multiplayer objects as unable to do so within ServerRpcs
    NetworkObject _previousSpawnedObject;
    string _playerName;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of MultiplayerManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(0, 1000).ToString());
    }

    /// <summary>
    /// Get the local player's name
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return _playerName;
    }

    /// <summary>
    /// Set the local player's name
    /// </summary>
    /// <param name="newName"></param>
    public void SetPlayerName(string newName)
    {
        _playerName = newName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, _playerName);////Temp for possible steam implementation
    }

    void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChange?.Invoke();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += SetConnectionApproval;
        NetworkManager.Singleton.OnConnectionEvent += Server_OnClientConnect;
        NetworkManager.Singleton.OnConnectionEvent += Server_OnClientDisconnect;
        NetworkManager.Singleton.StartHost();
    }

    void SetConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.SafeHouseScene.ToString())
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

        response.CreatePlayerObject = true;
        response.Approved = true;
    }

    void Server_OnClientConnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            AddPlayerToNetworkList(data.ClientId);

            //Init host data
            if (data.ClientId == NetworkManager.ServerClientId)
            {
                SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
                SetPlayerNameServerRpc(GetPlayerName());
            }
        }
    }

    void AddPlayerToNetworkList(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId
        });
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
        NetworkManager.Singleton.OnConnectionEvent += Client_OnLocalDisconnect;
        NetworkManager.Singleton.OnConnectionEvent += Client_OnLocalConnect;
        NetworkManager.Singleton.StartClient();
    }

    void Client_OnLocalDisconnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            OnLocalDisconnect?.Invoke();
        }
    }

    void Client_OnLocalConnect(NetworkManager manager, ConnectionEventData data)
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

    int GetPlayerDataIndexFromClientId(ulong clientId)
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
    /// Instantiates a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>The network object of the spawned object</returns>
    public NetworkObject SpawnObject(GameObject obj)
    {
        SpawnObjectServerRpc(GetIndexFromObject(obj));
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position">The position to spawn at</param>
    /// <param name="rotation">The rotation to spawn with</param>
    /// <returns></returns>
    public NetworkObject SpawnObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        SpawnObject(obj);
        _previousSpawnedObject.transform.position = position;
        _previousSpawnedObject.transform.rotation = rotation;
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="hand">The hand to set the items parents to</param>
    /// <returns></returns>
    public NetworkObject SpawnObject(GameObject obj, PlayerHand hand)
    {
        SpawnObjectWithParentServerRpc(GetIndexFromObject(obj), hand.GetNetworkObject());
        return _previousSpawnedObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectServerRpc(int objectIndex)
    {
        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        _previousSpawnedObject = networkObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectWithParentServerRpc(int objectIndex, NetworkObjectReference handNetworkObjectReference)
    {
        handNetworkObjectReference.TryGet(out NetworkObject handNetworkObject);

        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        PlayerHand hand = handNetworkObject.GetComponent<PlayerHand>();
        hand.SetItem(networkObject.GetComponent<Item>());

        _previousSpawnedObject = networkObject;
    }

    /// <summary>
    /// Destroy a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    public void DestroyObject(GameObject obj)
    {
        DestroyObjectServerRpc(obj.GetComponent<NetworkObject>(), 0);
    }

    /// <summary>
    /// Destroy a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="delay">The time delay to destroy this object</param>
    public void DestroyObject(GameObject obj, float delay)
    {
        DestroyObjectServerRpc(obj.GetComponent<NetworkObject>(), delay);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyObjectServerRpc(NetworkObjectReference networkObjectReference, float delay)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        Destroy(networkObject.gameObject, delay);
    }

    #endregion

}
