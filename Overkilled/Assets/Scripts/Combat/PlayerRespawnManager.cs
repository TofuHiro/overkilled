using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRespawnManager : NetworkBehaviour
{
    [Tooltip("The number of times required ")]
    [SerializeField] float _reviveMaxThreshold;
    [Tooltip("The value to add per interact attempt to revive progress")]
    [SerializeField] float _reviveTick;
    [Tooltip("The value to deduct per second to revive progress")]
    [SerializeField] float _reviveDecayPerSecond;

    public static PlayerRespawnManager Instance { get; private set; }

    Dictionary<PlayerHealth, float> _downedPlayersReviveProgress;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of PlayerRespawnManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _downedPlayersReviveProgress = new Dictionary<PlayerHealth, float>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerHealth.OnPlayerDowned += PlayerHealth_OnPlayerDowned;
        }   
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            PlayerHealth.OnPlayerDowned -= PlayerHealth_OnPlayerDowned;
        }
    }

    void PlayerHealth_OnPlayerDowned(PlayerHealth player)
    {
        _downedPlayersReviveProgress.Add(player, 0f);
        PlayerList.SetPlayerAlive(player.gameObject, false);
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (_downedPlayersReviveProgress.Count > 0)
            ReviveDecayTick();
    }

    void ReviveDecayTick()
    {
        foreach (var downedPlayer in _downedPlayersReviveProgress)
        {
            PlayerHealth player = downedPlayer.Key;

            float reviveProgress = _downedPlayersReviveProgress[player];
            reviveProgress -= Time.deltaTime * _reviveDecayPerSecond;
            reviveProgress = Mathf.Clamp(reviveProgress, 0f, _reviveMaxThreshold);
        }
    }

    public void ReviveTick(PlayerHealth player)
    {
        ReviveTickServerRpc(player.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    void ReviveTickServerRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        PlayerHealth player = playerNetworkObject.GetComponent<PlayerHealth>();

        if (!_downedPlayersReviveProgress.ContainsKey(player))
            return;

        _downedPlayersReviveProgress[player] += _reviveTick;

        if (_downedPlayersReviveProgress[player] >= _reviveMaxThreshold)
        {
            RevivePlayerClientRpc(playerNetworkObject);
            _downedPlayersReviveProgress.Remove(player);
            PlayerList.SetPlayerAlive(player.gameObject, true);
        }
    }

    [ClientRpc]
    void RevivePlayerClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        PlayerHealth player = playerNetworkObject.GetComponent<PlayerHealth>();

        player.RevivePlayer();
    }
}
