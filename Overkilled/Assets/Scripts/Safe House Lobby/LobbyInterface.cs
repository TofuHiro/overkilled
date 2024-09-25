using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInterface : MonoBehaviour
{
    public static LobbyInterface Instance { get; private set; }

    public bool IsInterfaceOpen { get; private set; }

    public event Action OnUICancel;

    PlayerController _currentPlayerInstance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyInterface found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        PlayerController.OnPlayerSpawn += Player_OnPlayerSpawn;

        _currentPlayerInstance = PlayerController.LocalInstance;
    }

    void Player_OnPlayerSpawn(PlayerController player)
    {
        _currentPlayerInstance = player;
        ToggleInterface(IsInterfaceOpen);
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerSpawn -= Player_OnPlayerSpawn;
        _currentPlayerInstance.OnUICancelInput -= Close;
    }

    public void ToggleInterface(bool state)
    {
        IsInterfaceOpen = state;

        _currentPlayerInstance.SetUIControls(state);
        if (state)
        {
            _currentPlayerInstance.OnUICancelInput += Close;
        }
        else
        {
            _currentPlayerInstance.OnUICancelInput -= Close;
        }
    }

    void Close()
    {
        OnUICancel?.Invoke();
    }
}
