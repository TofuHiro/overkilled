using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    const float READY_TOGGLE_COOLDOWN = 1f;

    public static CharacterSelectReady Instance { get; private set; }

    public event Action OnPlayerReadyChange;

    Dictionary<ulong, bool> _playerReadyDictionary;

    float _readyToggleCooldownTimer;
    bool _allPlayersReady;
    bool _checkPlayerReadySwitch;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of CharacterSelectReady found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _playerReadyDictionary = new Dictionary<ulong, bool>();    
    }

    void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += Network_OnConnectionEvent;
            //Add self/host to dictionary as OnConnectionEvent wont count for host
            _playerReadyDictionary.Add(NetworkManager.ServerClientId, false);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnConnectionEvent -= Network_OnConnectionEvent;
    }

    void Update()
    {
        if (_readyToggleCooldownTimer < READY_TOGGLE_COOLDOWN)
            _readyToggleCooldownTimer += Time.deltaTime;
    }

    void Network_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            _playerReadyDictionary.Add(data.ClientId, false);
            _allPlayersReady = false;
        }
        else if (data.EventType == ConnectionEvent.ClientDisconnected)
        {
            _playerReadyDictionary.Remove(data.ClientId);
            //Delayed CheckPlayerReady call to wait for dictionary to update
            _checkPlayerReadySwitch = true;
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

    /// <summary>
    /// Starts the game and loads the game scene
    /// </summary>
    public void StartGame()
    {
        if (!_allPlayersReady)
            return;

        GameLobby.Instance.DeleteLobby();
        Loader.LoadSceneNetwork(Loader.Scene.GameScene);
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
