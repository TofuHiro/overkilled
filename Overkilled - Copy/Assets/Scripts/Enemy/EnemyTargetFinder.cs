using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyTargetFinder : NetworkBehaviour
{
    [Header("Near Settings")]
    [Tooltip("The minimum distance to a player to be considered near. Starts using near poll rate")]
    [SerializeField] float _nearTargetDistanceThreshold = 5f;
    [Tooltip("The rate to poll for player locations when near one")]
    [SerializeField] float _nearTargetPollRate = .5f;
    [Header("Partway Settings")]
    [Tooltip("The minimum distance to a player to be considered partway. Starts using partway poll rate")]
    [SerializeField] float _partwayTargetDistanceThreshold = 10f;
    [Tooltip("The rate to poll for player locations when partway to one")]
    [SerializeField] float _partwayTargetPollRate = 1f;
    [Header("Far Settings")]
    [Tooltip("The rate to poll for player locations when far from one")]
    [SerializeField] float _farTargetPollRate = 3f;

    Dictionary<GameObject, bool> _alivePlayers = new Dictionary<GameObject, bool>();
    GameObject _closestPlayer;
    float _closestPlayerDistance = Mathf.Infinity;
    float _pollTimer = 0f, _nextTimeToPoll = 0f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        PlayerList.OnPlayerRemove += PlayerList_OnPlayerRemove; //In case players leave
        PlayerRespawnManager.Instance.OnPlayerRespawn += RespawnManager_OnPlayerRespawn;
        PlayerRespawnManager.Instance.OnPlayerDown += RespawnManager_OnPlayerDown;

        GameObject[] players = PlayerList.GetPlayers();
        foreach (var player in players)
        {
            _alivePlayers.Add(player, true);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        PlayerList.OnPlayerRemove -= PlayerList_OnPlayerRemove;
        PlayerRespawnManager.Instance.OnPlayerRespawn -= RespawnManager_OnPlayerRespawn;
        PlayerRespawnManager.Instance.OnPlayerDown -= RespawnManager_OnPlayerDown;

        _alivePlayers.Clear();
    }

    void PlayerList_OnPlayerRemove(GameObject player)
    {
        _alivePlayers.Remove(player);
    }

    void RespawnManager_OnPlayerRespawn(GameObject player)
    {
        _alivePlayers[player] = true;
    }

    void RespawnManager_OnPlayerDown(GameObject player)
    {
        _alivePlayers[player] = false;
        PollClosestPlayer();
    }

    /// <summary>
    /// Get the current closest player to this enemy
    /// </summary>
    /// <returns></returns>
    public GameObject GetClosestPlayer()
    {
        return _closestPlayer;
    }

    void Update()
    {
        if (!IsServer)
            return;

        _pollTimer += Time.deltaTime;

        if (_pollTimer >= _nextTimeToPoll)
        {
            PollClosestPlayer();
            _nextTimeToPoll = GetPollRate(_closestPlayerDistance);
            _pollTimer = 0f;
        }
    }

    void PollClosestPlayer()
    {
        if (_alivePlayers.Count == 0)
        {
            _closestPlayer = null;
            return;
        }

        foreach (var player in _alivePlayers)
        {
            //Ignore dead
            if (player.Value == false) 
                continue;

            float distance = Vector3.Distance(player.Key.transform.position, transform.position);
            if (distance < _closestPlayerDistance)
            {
                _closestPlayer = player.Key;
                _closestPlayerDistance = distance;
            }
        }
    }

    /// <summary>
    /// Get the variable poll rate based on the current closest player's distance.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    float GetPollRate(float distance)
    {
        if (distance > _partwayTargetDistanceThreshold)
            return _farTargetPollRate;
        else if (distance > _nearTargetDistanceThreshold)
            return _partwayTargetPollRate;
        else
            return _nearTargetPollRate;
    }
}
