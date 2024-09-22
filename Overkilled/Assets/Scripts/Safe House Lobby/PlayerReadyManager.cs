using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerReadyManager : NetworkBehaviour
{
    const float READY_TOGGLE_COOLDOWN = 1f;

    public static PlayerReadyManager Instance { get; private set; }

    public bool AllPlayersReady {  get {  return _allPlayersReady; } }

    public event Action OnPlayerReadyChange;

    Dictionary<ulong, bool> _playerReadyDictionary;
    float _readyToggleCooldownTimer;
    bool _allPlayersReady;
    bool _checkPlayerReadySwitch;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of PlayerReadyManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    void Start()
    {
        LobbyManager.Instance.OnSwitchToMultiplayer += LobbyManager_OnSwitchToMultiplayer;
    }

    void LobbyManager_OnSwitchToMultiplayer()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += NetworkManager_Server_OnConnectionEvent;
            _playerReadyDictionary.Add(NetworkManager.ServerClientId, false);
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= NetworkManager_Server_OnConnectionEvent;
    }

    void Update()
    {
        if (_readyToggleCooldownTimer < READY_TOGGLE_COOLDOWN)
            _readyToggleCooldownTimer += Time.deltaTime;
    }

    void NetworkManager_Server_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            _playerReadyDictionary.Add(data.ClientId, false);
            _allPlayersReady = false;

            SyncDictionaryServerRpc();
        }
        else if (data.EventType == ConnectionEvent.ClientDisconnected)
        {
            _playerReadyDictionary.Remove(data.ClientId);
            //Delayed CheckPlayerReady call to wait for dictionary to update
            _checkPlayerReadySwitch = true;
        }
    }

    [ServerRpc]
    void SyncDictionaryServerRpc()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SetPlayerReadyClientRpc(_playerReadyDictionary[clientId], clientId);
        }
    }

    void LateUpdate()
    {
        if (_checkPlayerReadySwitch)
        {
            CheckPlayerReady();
            _checkPlayerReadySwitch = false;
        }
    }

    /// <summary>
    /// Toggles the player's ready state
    /// </summary>
    public void TogglePlayerReady()
    {
        if (_readyToggleCooldownTimer < READY_TOGGLE_COOLDOWN)
            return;

        TogglePlayerReadyServerRpc();
        _readyToggleCooldownTimer = 0f;
    }

    [ServerRpc(RequireOwnership = false)]
    void TogglePlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool state = !_playerReadyDictionary[serverRpcParams.Receive.SenderClientId];

        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = state;
        SetPlayerReadyClientRpc(state, serverRpcParams.Receive.SenderClientId);

        CheckPlayerReady();
    }

    /// <summary>
    /// Checks and sets the AllPlayersReady bool
    /// </summary>
    void CheckPlayerReady()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                _allPlayersReady = false;
                return;
            }
        }

        _allPlayersReady = true;
    }

    [ClientRpc]
    void SetPlayerReadyClientRpc(bool state, ulong clientId)
    {
        _playerReadyDictionary[clientId] = state;

        OnPlayerReadyChange?.Invoke();
    }

    /// <summary>
    /// Check if a player is ready or not
    /// </summary>
    /// <param name="clientId">The client id of the player</param>
    /// <returns></returns>
    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}
