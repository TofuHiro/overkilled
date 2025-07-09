using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static MultiplayerManager Instance { get; private set; }

    /// <summary>
    /// Invoked when the local player is disconnect
    /// </summary>
    public event Action OnLocalDisconnect;
    public event Action<string> OnLobbyReloadAfterDisconnect;
    public event Action OnPlayerDataNetworkListChange;

    NetworkList<PlayerData> _playerDataNetworkList;
    string _playerName;
    string _clientDisconnectReason;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of MultiplayerManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(0, 1000).ToString());
    }

    /// <summary>
    /// Get the local player's name
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName() { return _playerName; }

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

        _playerDataNetworkList.Clear();
        AddPlayerToNetworkList(NetworkManager.ServerClientId);
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        SetPlayerNameServerRpc(GetPlayerName());
    }

    void SetConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.SafeHouseScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= GameLobby.Instance.GetLobby().MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Lobby is full";
            return;
        }

        response.Approved = true;
    }

    void Server_OnClientConnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            AddPlayerToNetworkList(data.ClientId);
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            SetPlayerNameServerRpc(GetPlayerName());
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

    async void Client_OnLocalDisconnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            try
            {
                OnLocalDisconnect?.Invoke();

                //Set disconnect reason to show after client reloads lobby
                _clientDisconnectReason = manager.DisconnectReason;
                LobbyManager.OnLobbyLoad += LobbyManager_OnLobbyLoad;

                await LeaveMultiplayer();

                Loader.Instance.LoadScene(Loader.Scene.SafeHouseScene, Loader.TransitionType.FadeOut, Loader.TransitionType.FadeIn);
            }
            catch (Exception e)
            {
                Debug.LogError("Error trying to leave multiplayer" + "\n" + e);
            }
        }
    }

    void LobbyManager_OnLobbyLoad()
    {
        OnLobbyReloadAfterDisconnect?.Invoke(_clientDisconnectReason);
        LobbyManager.OnLobbyLoad -= LobbyManager_OnLobbyLoad;
    }

    void Client_OnLocalConnect(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
        {
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            SetPlayerNameServerRpc(GetPlayerName());
        }
    }

    public bool IsPlayerIndexConnected(int index)
    {
        return index < _playerDataNetworkList.Count;
    }

    public async Task LeaveMultiplayer()
    {
        try
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= SetConnectionApproval;
            NetworkManager.Singleton.OnConnectionEvent -= Server_OnClientConnect;
            NetworkManager.Singleton.OnConnectionEvent -= Server_OnClientDisconnect;

            NetworkManager.Singleton.OnConnectionEvent -= Client_OnLocalDisconnect;
            NetworkManager.Singleton.OnConnectionEvent -= Client_OnLocalConnect;

            if (IsServer)
                HostDisconnect();

            if (NetworkManager.IsClient)
                NetworkManager.Singleton.Shutdown();

            await GameLobby.Instance.LeaveLobby();
        }
        catch (Exception e)
        {
            Debug.LogError("Error trying to leave multiplaye" + "\n" + e);
        }
    }

    public void HostDisconnect()
    {
        ulong[] clientIds = NetworkManager.ConnectedClientsIds.ToArray();
        foreach (ulong clientId in clientIds)
            if (clientId != NetworkManager.ServerClientId)
                KickPlayer(clientId, "Host has disconnected");
    }

    public void KickPlayer(ulong clientId, string reason)
    {
        NetworkManager.Singleton.DisconnectClient(clientId, reason);
        RemovePlayerData(clientId);
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


    void OnApplicationQuit()
    {
        if (IsServer)
            HostDisconnect();
    }

    void Update()
    {
        if (IsServer && NetworkManager.ShutdownInProgress)
            HostDisconnect();
    }

}
