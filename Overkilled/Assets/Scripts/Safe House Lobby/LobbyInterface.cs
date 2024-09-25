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
        PlayerController.OnSingletonSwitch += PlayerController_OnSingletonSwitch;

        _currentPlayerInstance = PlayerController.LocalInstance;
    }

    void OnDestroy()
    {
        PlayerController.OnSingletonSwitch -= PlayerController_OnSingletonSwitch;
        _currentPlayerInstance.OnUICancelInput -= Close;
    }

    void PlayerController_OnSingletonSwitch(PlayerController prevRef, PlayerController newRef)
    {
        prevRef.SetUIControls(false);
        prevRef.OnUICancelInput -= Close;

        _currentPlayerInstance = newRef;

        if (IsInterfaceOpen)
        {
            newRef.SetUIControls(true);
            newRef.OnUICancelInput += Close;
        }
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
