using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [Tooltip("The level this button will set the selected level to")]
    [SerializeField] Loader.Level _level;
    [Tooltip("The locked interface to display when this level is locked")]
    [SerializeField] GameObject _lockedOverlay;
    [Tooltip("The UI element to display that this level is currently selected")]
    [SerializeField] GameObject _selectedOverlay;

    Button _button;
    bool _isLocked = false;

    void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() =>
        {
            if (_isLocked)
                return;

            LobbyManager.Instance.SetLevel(_level);
        });
    }

    void Start()
    {
        LobbyManager.Instance.OnLevelChange += LobbyManager_OnLevelChange;
        LobbyManager.Instance.OnSwitchToMultiplayer += LobbyManager_OnSwitchToMultiplayer;
        GameLobby.Instance.OnCreateLobbySuccess += ToggleButton;
        GameLobby.Instance.OnJoinSuccess += ToggleButton;

        _selectedOverlay.SetActive(false);
    }

    void OnDestroy()
    {
        LobbyManager.Instance.OnLevelChange -= LobbyManager_OnLevelChange;
        LobbyManager.Instance.OnSwitchToMultiplayer -= LobbyManager_OnSwitchToMultiplayer;
        GameLobby.Instance.OnCreateLobbySuccess -= ToggleButton;
        GameLobby.Instance.OnJoinSuccess -= ToggleButton;
    }

    /// <summary>
    /// Client side display
    /// </summary>
    /// <param name="level"></param>
    void LobbyManager_OnLevelChange(Loader.Level level)
    {
        if (level == _level)
        {
            _selectedOverlay.SetActive(true);
        }
        else
        {
            _selectedOverlay.SetActive(false);
        }
    }

    void LobbyManager_OnSwitchToMultiplayer(bool isHost)
    {
        _button.enabled = isHost;
    }


    void ToggleButton()
    {
        _button.enabled = NetworkManager.Singleton.IsServer;
    }

    public void ToggleLock(bool state)
    {
        _isLocked = state;
        _lockedOverlay.SetActive(state);
    }
}
