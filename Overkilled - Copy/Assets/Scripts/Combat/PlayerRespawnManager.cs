using SurvivalGame;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRespawnManager : NetworkBehaviour
{
    [Tooltip("The threshold value to reach to revive a player")]
    [SerializeField] float _reviveMaxThreshold;
    [Tooltip("The value to add per interact attempt to revive progress")]
    [SerializeField] float _reviveTick;
    [Tooltip("The value to deduct per second to revive progress")]
    [SerializeField] float _reviveDecayPerSecond;

    public static PlayerRespawnManager Instance { get; private set; }

    public delegate void PlayerRespawnAction(GameObject player);
    public PlayerRespawnAction OnPlayerDown, OnPlayerRespawn;

    /// <summary>
    /// Invoked when all players are downed/dead simultaneously
    /// </summary>
    public event Action OnAllPlayersDead;

    Dictionary<PlayerHealth, float> _downedPlayersReviveProgress = new Dictionary<PlayerHealth, float>();
    Dictionary<PlayerHealth, bool> _downedPlayers = new Dictionary<PlayerHealth, bool>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of PlayerRespawnManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        PlayerHealth.OnPlayerDowned += PlayerHealth_OnPlayerDowned;
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        PlayerHealth.OnPlayerDowned -= PlayerHealth_OnPlayerDowned;
        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.IsStarting)
        {
            foreach (GameObject player in PlayerList.GetPlayers())
            {
                _downedPlayers.Add(player.GetComponent<PlayerHealth>(), false);
            }
        }
    }

    void PlayerHealth_OnPlayerDowned(PlayerHealth downedPlayer)
    {
        _downedPlayersReviveProgress[downedPlayer] = 0f;
        _downedPlayers[downedPlayer] = true;

        bool allPlayersDown = true;
        foreach (var player in _downedPlayers)
            if (player.Value == false)
                allPlayersDown = false;
            
        if (allPlayersDown)
            OnAllPlayersDead?.Invoke();
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (_reviveDecayPerSecond <= 0)
            return;

        if (_downedPlayersReviveProgress.Count > 0)
            ReviveDecayTick();
    }

    void ReviveDecayTick()
    {
        foreach (PlayerHealth downedPlayer in _downedPlayersReviveProgress.Keys)
        {
            if (_downedPlayersReviveProgress[downedPlayer] > 0f)
                _downedPlayersReviveProgress[downedPlayer] -= _reviveDecayPerSecond * Time.deltaTime;  
            else
                _downedPlayersReviveProgress[downedPlayer] = 0f;
        }
    }

    public void ReviveTick(PlayerHealth player)
    {
        int playerListIndex = PlayerList.GetPlayerListIndex(player.gameObject);
        ReviveTickServerRpc(playerListIndex);
    }

    [Rpc(SendTo.Server)]
    void ReviveTickServerRpc(int playerListIndex)
    {
        GameObject playerObject = PlayerList.GetPlayer(playerListIndex);
        PlayerHealth player = playerObject.GetComponent<PlayerHealth>();

        if (!_downedPlayersReviveProgress.ContainsKey(player))
            return;

        _downedPlayersReviveProgress[player] += _reviveTick;

        if (_downedPlayersReviveProgress[player] >= _reviveMaxThreshold)
        {
            player.RevivePlayer();
            _downedPlayersReviveProgress.Remove(player);
            _downedPlayers[player] = false;
        }
    }
}
