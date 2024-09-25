using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    [Tooltip("The local player reference the local player starts with")]
    [SerializeField] PlayerController _localPlayerInstance;
    [Tooltip("The position to spawn the local player when the scene starts")]
    [SerializeField] Vector3 _localPlayerSpawnPosition;

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

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;

        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += NetworkManager_Server_OnConnectionEvent;
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_OnConnectionEvent;

        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_Server_OnConnectionEvent;
        }
    }

    void NetworkManager_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.ClientId == NetworkManager.LocalClientId)
        {
            if (data.EventType == ConnectionEvent.ClientConnected)
            {
                _localPlayerInstance.gameObject.SetActive(false);
                OnSwitchToMultiplayer?.Invoke(NetworkManager.IsServer);
            }
        }
    }

    void NetworkManager_Server_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            SpawnPlayer(data.ClientId);
        }
    }

    void SpawnPlayer(ulong clientId)
    {
        GameObject playerObject = Instantiate(_playerPrefab);
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
