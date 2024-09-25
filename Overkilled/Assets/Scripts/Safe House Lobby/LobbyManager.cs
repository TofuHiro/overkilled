using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [Tooltip("The player prefab to spawn as a player")]
    [SerializeField] GameObject _playerPrefab;

    public static LobbyManager Instance { get; private set; }

    public delegate void LevelSelectAction(Loader.Level level);
    public event LevelSelectAction OnLevelChange;
    public delegate void LobbyAction(bool isHost);
    public event LobbyAction OnSwitchToMultiplayer;

    Loader.Level _selectedLevel;

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
        GameLobby.Instance.OnCreateLobbySuccess += MultiplayerSwitch;
        GameLobby.Instance.OnJoinSuccess += MultiplayerSwitch;

        //Local network - Unity Transport
        NetworkManager.Singleton.StartHost();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += SpawnClient;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= SpawnClient;
        }
    }

    void MultiplayerSwitch()
    {
        OnSwitchToMultiplayer?.Invoke(NetworkManager.IsServer);
    }

    void SpawnClient(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            GameObject playerObject = Instantiate(_playerPrefab);
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(data.ClientId, true);
        }
    }

    public void EndLocalHost()
    {
        NetworkManager.Singleton.Shutdown();
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

        GameLobby.Instance.DeleteLobby();

        Loader.LoadLevel(_selectedLevel);
    }

    public void ReloadLobby()
    {
        Destroy(NetworkManager.Singleton.gameObject);
        Destroy(MultiplayerManager.Instance.gameObject);

        SceneManager.LoadScene(Loader.Scene.SafeHouseScene.ToString());
    }

    public void SetLevel(Loader.Level level)
    {
        _selectedLevel = level;
        OnLevelChange?.Invoke(_selectedLevel);
        SetLevelRpc(_selectedLevel);
    }

    [Rpc(SendTo.NotMe)]
    void SetLevelRpc(Loader.Level level)
    {
        _selectedLevel = level;
        OnLevelChange?.Invoke(level);
    }
}
