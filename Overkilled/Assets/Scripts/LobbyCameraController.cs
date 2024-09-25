using Unity.Netcode;
using UnityEngine;

public class LobbyCameraController : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [Tooltip("The smooth time for camera movement")]
    [SerializeField] float _smoothTime;

    [Header("Focus Mode Settings")]
    [Tooltip("The multiplier applied to camera offset")]
    [SerializeField] float _offsetMultiplier;

    Transform _player;
    Vector3 _startOffset;
    Vector3 _velocity = Vector3.zero;

    void Start()
    {
        PlayerController.OnPlayerSpawn += Player_OnPlayerSpawn;

        _startOffset = transform.position;
        _player = PlayerController.LocalInstance.transform;
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerSpawn -= Player_OnPlayerSpawn;
    }

    void Player_OnPlayerSpawn(PlayerController player)
    {
        _player = player.transform;
    }

    void Update()
    {
        if (_player == null)
            return;

        MoveCamera();
    }

    void MoveCamera()
    {
        _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _startOffset + (_player.position * _offsetMultiplier), ref _velocity, _smoothTime);
    }
}
