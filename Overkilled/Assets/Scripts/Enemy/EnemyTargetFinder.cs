using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetFinder : MonoBehaviour
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

    GameObject _closestPlayer;
    GameObject[] _playerReferences;
    float _closestPlayerDistance;
    float _pollTimer = 0f, _nextTimeToPoll = 0f;

    void Start()
    {
        PlayerList.OnPlayerListUpdate += PlayerList_OnPlayerListUpdate;
        PlayerList.OnPlayerAliveUpdate += PlayerList_OnPlayerAliveUpdate;

        SetPlayerReferences();
    }

    void PlayerList_OnPlayerListUpdate()
    {
        SetPlayerReferences();
        PollClosestPlayer();
    }

    void PlayerList_OnPlayerAliveUpdate()
    {
        SetPlayerReferences();
        PollClosestPlayer();
    }

    void OnDestroy()
    {
        PlayerList.OnPlayerListUpdate -= PlayerList_OnPlayerListUpdate;
        PlayerList.OnPlayerAliveUpdate -= PlayerList_OnPlayerAliveUpdate;
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
        _pollTimer += Time.deltaTime;
        if (_pollTimer >= _nextTimeToPoll)
        {
            PollClosestPlayer();
            _nextTimeToPoll = GetPollRate(_closestPlayerDistance);
            _pollTimer = 0f;
        }
    }

    void SetPlayerReferences()
    {
        _playerReferences = PlayerList.GetAlivePlayers();
    }

    void PollClosestPlayer()
    {
        if (_playerReferences.Length == 0)
        {
            _closestPlayer = null;
            return;
        }

        _closestPlayer = _playerReferences[0];
        _closestPlayerDistance = Vector3.Distance(_playerReferences[0].transform.position, transform.position);

        for (int i = 1; i < _playerReferences.Length; i++)
        {
            float distance = Vector3.Distance(_playerReferences[i].transform.position, transform.position);
            if (distance < _closestPlayerDistance)
            {
                _closestPlayer = _playerReferences[i];
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
