using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMenuUI : MonoBehaviour
{
    [Tooltip("Button on the taskbar to open the quick menu")]
    [SerializeField] Button _taskbarMenuButton;
    [Tooltip("The hidden overlay button in the background when the quick menu is open where if pressed, will close the quick menu")]
    [SerializeField] Button _exitMenuBackgroundButton;
    [Tooltip("Exit to menu button in the quick menu")]
    [SerializeField] Button _exitToMenuButton;
    [Tooltip("Host Lobby Button in the quick menu")]

    bool _isOpen;

    void Awake()
    {
        _taskbarMenuButton.onClick.AddListener(ToggleMenu);

        _exitMenuBackgroundButton.onClick.AddListener(Hide);

        _exitToMenuButton.onClick.AddListener(LeaveToMenu);
    }

    void Start()
    {
        Hide();
    }

    async void LeaveToMenu()
    {
        try
        {
            await MultiplayerManager.Instance.LeaveMultiplayer();

            Loader.LoadScene(Loader.Scene.MainMenuScene);
        }
        catch (Exception e)
        {
            Debug.LogError("Error trying to leave lobby to menu" + "\n" + e);
        }
    }

    void ToggleMenu()
    {
        _isOpen = !_isOpen;

        if (_isOpen)
            Show();
        else
            Hide();
    }

    void Show()
    {
        _isOpen = true;
        gameObject.SetActive(true);
        _exitMenuBackgroundButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isOpen = false;
        gameObject.SetActive(false);
        _exitMenuBackgroundButton.gameObject.SetActive(false);
    }
}
