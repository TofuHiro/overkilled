using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMultiplayerController : NetworkBehaviour
{
    PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public override void OnNetworkSpawn()
    {
        PlayerList.AddPlayer(gameObject);

        InitializeOwner();
    }

    public override void OnNetworkDespawn()
    {
        PlayerList.RemovePlayer(gameObject);
    }

    void InitializeOwner()
    {
        if (IsOwner)
        {
            _playerController.InitSingleton(_playerController);
        }
        else
        {
            _playerController.SetCanControl(false);
        }
    }
}
