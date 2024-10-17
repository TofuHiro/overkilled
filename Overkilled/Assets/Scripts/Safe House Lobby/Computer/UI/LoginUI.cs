using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Tooltip("The button to submit the player's name")]
    [SerializeField] Button _loginButton;
    [Tooltip("The input field to enter the player's name")]
    [SerializeField] TMP_InputField _playerNameInputField;
    [Tooltip("The taskbar object to show after logging in")]
    [SerializeField] GameObject _taskBar;

    void Awake()
    {
        _loginButton.onClick.AddListener(Hide);

        /*_playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            MultiplayerManager.Instance.SetPlayerName(newText);
        });*/
    }

    void Start()
    {
        if (GameLobby.Instance.InLobby)
        {
            Hide();
        }
        else
        {
            _playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();

            Show();
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
        _taskBar.SetActive(false);
    }

    void Hide()
    {
        gameObject.SetActive(false);
        _taskBar.SetActive(true);
    }
}
