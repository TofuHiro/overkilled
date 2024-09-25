using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] Loader.Level _level;
    [SerializeField] GameObject _lockedOverlay;
    [SerializeField] GameObject _selectedOverlay;

    static GameObject s_currentSelectedOverlay;

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

            SetSelectedOverlay();
        });
    }

    void Start()
    {
        LobbyManager.Instance.OnSwitchToMultiplayer += LobbyManager_OnSwitchToMultiplayer;
        LobbyManager.Instance.OnLevelChange += LobbyManager_Client_OnLevelChange;

        _selectedOverlay.SetActive(false);
    }

    void LobbyManager_OnSwitchToMultiplayer(bool isHost)
    {
        _button.enabled = isHost;
    }

    /// <summary>
    /// Client side display
    /// </summary>
    /// <param name="level"></param>
    void LobbyManager_Client_OnLevelChange(Loader.Level level)
    {
        if (level.ToString() == _level.ToString())
        {
            SetSelectedOverlay();
        }
    }

    void SetSelectedOverlay()
    {
        if (s_currentSelectedOverlay != null)
            s_currentSelectedOverlay.SetActive(false);

        s_currentSelectedOverlay = _selectedOverlay;
        s_currentSelectedOverlay.SetActive(true);
    }

    public Loader.Level GetLevel() { return _level; }

    public void ToggleLock(bool state)
    {
        _isLocked = state;
        _lockedOverlay.SetActive(state);
    }
}
