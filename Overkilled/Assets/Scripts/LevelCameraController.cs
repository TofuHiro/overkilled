using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Presets;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    public static LevelCameraController Instance { get; private set; }

    [Tooltip("The camera to offset")]
    [SerializeField] Camera _camera;
    [Tooltip("The smooth time for camera movement")]
    [SerializeField] float _smoothTime;
    [Tooltip("Whether to use camera follow focus mode or fix mode")]
    [SerializeField] bool _useFocusMode = true;

    [Header("Focus Mode Settings")]
    [Tooltip("The multiplier applied to camera offset")]
    [SerializeField] float _offsetMultiplier;

    [Header("Fixed Mode Settings")]
    [Tooltip("The position to calculate pan out offsets with the player positions from")]
    [SerializeField] Vector3 _playerStartPosition;
    [Tooltip("The multiplier applied to pan out offsets")]
    [SerializeField] float _panOutMultiplier;

    GameObject[] _players;
    Vector3 _startOffset;
    Vector3 _avgPlayerPos = Vector3.zero;
    Vector3 _velocity = Vector3.zero;
    float _furthestPlayerDist = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LevelCameraController found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        PlayerList.OnPlayerListUpdate += SetPlayers;

        _startOffset = transform.position;
        SetPlayers();
    }

    void OnDestroy()
    {
        PlayerList.OnPlayerListUpdate -= SetPlayers;
    }

    void SetPlayers()
    {
        _players = PlayerList.GetPlayers();
    }

    public void SetFocusMode(bool state)
    {
        SetFocusModeServerRpc(state);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetFocusModeServerRpc(bool state)
    {
        SetFocusModeClientRpc(state);
    }
    [ClientRpc]
    void SetFocusModeClientRpc(bool state)
    {
        _useFocusMode = state;
    }

    void Update()
    {
        if (_players == null || _players.Length == 0)
            return;

        if (_useFocusMode)
        {
            SetAvgPlayerPosition();
            MoveCamera();
        }
        else
        {
            SetFurthestPlayerDistance();
            PanCamera();
        }
    }

    void SetAvgPlayerPosition()
    {
        Vector3 positionSum = Vector3.zero;
        foreach (var player in _players)
            positionSum += player.transform.position;

        _avgPlayerPos = positionSum / _players.Length;
    }
    
    void MoveCamera()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _startOffset + (_avgPlayerPos * _offsetMultiplier), ref _velocity, _smoothTime);
    }

    void SetFurthestPlayerDistance()
    {
        _furthestPlayerDist = Vector3.Distance(_playerStartPosition, _players[0].transform.position);
        for (int i = 1; i < _players.Length; i++)
        {
            float distance = Vector3.Distance(_playerStartPosition, _players[i].transform.position);
            if (distance > _furthestPlayerDist)
            {
                _furthestPlayerDist = distance;
            }
        }
    }

    void PanCamera()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _startOffset + (-_camera.transform.forward * _furthestPlayerDist * _panOutMultiplier), ref _velocity, _smoothTime);
    }
}
