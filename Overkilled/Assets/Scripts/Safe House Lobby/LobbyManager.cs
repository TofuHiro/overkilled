using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [Tooltip("The player prefab to spawn as a player")]
    [SerializeField] GameObject _playerPrefab;

    public static LobbyManager Instance { get; private set; }

    public delegate void LevelSelectAction(Loader.Level level);
    /// <summary>
    /// Invoked when the selected level is changed
    /// </summary>
    public event LevelSelectAction OnLevelChange;

    public delegate void LobbyAction(bool isHost);
    /// <summary>
    /// Invoked when the lobby switches from local host to multiplayer. This can be when the player creates a new lobby or join a lobby
    /// </summary>
    public event LobbyAction OnSwitchToMultiplayer;

    public static event Action OnLobbyLoad;

    Loader.Level _selectedLevel;
    Vector3 _currentPlayerPosition;
    Quaternion _currentPlayerRotation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnCreateLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += MultiplayerSwitch;

        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;

        GameLobby.Instance.OnCreateLobbyStarted += EndLocalHost;
        GameLobby.Instance.OnJoinStarted += EndLocalHost;

        if (!GameLobby.Instance.InLobby)
        {
            //Local network - Unity Transport
            StartLocalHost();
        }
        else
        {
            if (IsServer)
            {
                //Spawn all players
                foreach (ulong cliendId in NetworkManager.ConnectedClientsIds)
                    SpawnClient(cliendId);
            }
        }

        OnLobbyLoad?.Invoke();
    }

    public override void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnCreateLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= MultiplayerSwitch;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
        GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnCreateLobbyStarted -= EndLocalHost;
        GameLobby.Instance.OnJoinStarted -= EndLocalHost;
    }

    void GameLobby_OnCreateLobbySuccess()
    {
        MultiplayerSwitch();

        PlayerController.LocalInstance.SetPositionAndRotation(_currentPlayerPosition, _currentPlayerRotation);
    }

    void GameLobby_OnCreateLobbyFailed()
    {
        StartLocalHost();

        PlayerController.LocalInstance.SetPositionAndRotation(_currentPlayerPosition, _currentPlayerRotation);
    }

    void GameLobby_OnJoinFailed()
    {
        StartLocalHost();

        PlayerController.LocalInstance.SetPositionAndRotation(_currentPlayerPosition, _currentPlayerRotation);
    }

    void GameLobby_OnQuickJoinFailed()
    {
        StartLocalHost();

        PlayerController.LocalInstance.SetPositionAndRotation(_currentPlayerPosition, _currentPlayerRotation);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_OnConnectionEvent;
        }
    }
    
    void StartLocalHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);

        NetworkManager.Singleton.StartHost();
    }

    void EndLocalHost()
    {
        NetworkManager.Singleton.Shutdown();

        _currentPlayerPosition = PlayerController.LocalInstance.transform.position;
        _currentPlayerRotation = PlayerController.LocalInstance.transform.rotation;
    }

    void MultiplayerSwitch()
    {
        OnSwitchToMultiplayer?.Invoke(NetworkManager.IsServer);
    }

    void NetworkManager_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            SpawnClient(data.ClientId);
        }
    }

    void SpawnClient(ulong clientId)
    {
        GameObject playerObject = Instantiate(_playerPrefab, PlayerSpawnManager.Instance.GetNextSpawnPosition(), Quaternion.identity);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    public void StartGame()
    {
        if (!PlayerReadyManager.Instance.AllPlayersReady)
        {
            Debug.Log("All players are not ready");
            return;
        }
        if (_selectedLevel == Loader.Level.None)
        {
            Debug.Log("Level not selected");
            return;
        }

        if (IsServer)
            GameLobby.Instance.LockLobby();
        MultiplayerManager.Instance.SetCurrentLevel(_selectedLevel);
        Loader.LoadLevel(_selectedLevel);
    }

    public void StartSoloGame()
    {
        if (_selectedLevel == Loader.Level.None)
        {
            Debug.Log("Level not selected");
            return;
        }

        MultiplayerManager.Instance.SetCurrentLevel(_selectedLevel);
        Loader.LoadLevel(_selectedLevel);
    }

    public async void LeaveLobby()
    {
        try
        {
            await MultiplayerManager.Instance.LeaveMultiplayer();
            ReloadLobby();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Reloads the lobby scene, reverting everything to its defaults and local host
    /// </summary>
    public void ReloadLobby()
    {
        Loader.LoadScene(Loader.Scene.SafeHouseScene);
    }

    /// <summary>
    /// Get the selected level
    /// </summary>
    /// <returns></returns>
    public Loader.Level GetSelectedLevel()
    {
        return _selectedLevel;
    }

    /// <summary>
    /// Set the selected level to a given level
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(Loader.Level level)
    {
        _selectedLevel = level;
        OnLevelChange?.Invoke(_selectedLevel);

        if (IsServer)
            SetLevelRpc(_selectedLevel);
    }

    [Rpc(SendTo.NotMe)]
    void SetLevelRpc(Loader.Level level)
    {
        _selectedLevel = level;
        OnLevelChange?.Invoke(level);
    }
}
