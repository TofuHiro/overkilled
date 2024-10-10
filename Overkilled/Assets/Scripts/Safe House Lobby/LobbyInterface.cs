using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInterface : MonoBehaviour
{
    public static LobbyInterface Instance { get; private set; }

    /// <summary>
    /// Whether a UI is open or not
    /// </summary>
    public bool IsInterfaceOpen { get; private set; }

    /// <summary>
    /// Invoked when local player toggles menu button
    /// </summary>
    public event Action OnMenuToggle;

    /// <summary>
    /// Invoked when local player cancel action is invoked
    /// </summary>
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
        _currentPlayerInstance.OnUICancelInput += PlayerController_OnUICancelInput;
        _currentPlayerInstance.OnMenuInput += PlayerController_OnMenuInput;
        ToggleInterface(IsInterfaceOpen);
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerSpawn -= Player_OnPlayerSpawn;
        _currentPlayerInstance.OnUICancelInput -= PlayerController_OnUICancelInput;
        _currentPlayerInstance.OnMenuInput -= PlayerController_OnMenuInput;
    }

    /// <summary>
    /// Toggle interface settings. If true, player UI controls are activated.
    /// </summary>
    /// <param name="state"></param>
    public void ToggleInterface(bool state)
    {
        IsInterfaceOpen = state;

        _currentPlayerInstance.SetUIControls(state);
    }

    void PlayerController_OnUICancelInput()
    {
        OnUICancel?.Invoke();
    }

    void PlayerController_OnMenuInput()
    {
        OnMenuToggle?.Invoke();
    }
}
