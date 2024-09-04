using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCameraController : MonoBehaviour
{
    [Tooltip("The camera to offset")]
    [SerializeField] Camera _camera;
    [Tooltip("The multiplier applied to camera offset")]
    [SerializeField] float _offsetMultiplier;
    [Tooltip("The smooth time for camera movement")]
    [SerializeField] float _smoothTime;

    GameObject[] _players;
    Vector3 _startOffset;
    Vector3 _avgPlayerPos = Vector3.zero;
    Vector3 _velocity = Vector3.zero;

    void Start()
    {
        _startOffset = transform.position;

        PlayerList.OnPlayerListUpdate += SetPlayers;
        SetPlayers();
    }

    void SetPlayers()
    {
        _players = PlayerList.GetPlayers();
    }

    void Update()
    {
        if (_players == null || _players.Length == 0)
            return;

        SetAvgPlayerPosition();
        MoveCamera();
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
}
