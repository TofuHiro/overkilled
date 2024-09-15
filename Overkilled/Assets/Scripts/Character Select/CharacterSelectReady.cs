using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    const float READY_TOGGLE_COOLDOWN = 1f;

    public static CharacterSelectReady Instance;

    public event Action OnPlayerReadyChange;

    Dictionary<ulong, bool> _playerReadyDictionary;

    float _readyToggleCooldownTimer;
    bool _allPlayersReady;
    bool _checkPlayerReadySwitch;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
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
        {
            _readyToggleCooldownTimer += Time.deltaTime;
        }
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

    public void TogglePlayerReady()
    {
        if (_readyToggleCooldownTimer < READY_TOGGLE_COOLDOWN)
            return;

        TogglePlayerReadyServerRpc();
        _readyToggleCooldownTimer = 0f;
    }

    /// <summary>
    /// Checks if all players in a server are ready and loads the game scene
    /// </summary>
    /// <param name="serverRpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    void TogglePlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool state = !_playerReadyDictionary[serverRpcParams.Receive.SenderClientId];

        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = state;
        SetPlayerReadyClientRpc(state, serverRpcParams.Receive.SenderClientId);

        CheckPlayerReady();
    }

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

    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}
