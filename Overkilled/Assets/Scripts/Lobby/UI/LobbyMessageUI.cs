using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [Tooltip("Text to display lobby messages")]
    [SerializeField] TMP_Text _messageText;
    [Tooltip("Button to close message")]
    [SerializeField] Button _closeButton;

    void Awake()
    {
        _closeButton.onClick.AddListener(Hide);
    }

    void Start()
    {
        MultiplayerManager.Instance.OnLocalDisconnect += MultiplayerManager_OnFailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.Instance.OnLocalDisconnect -= MultiplayerManager_OnFailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
    }

    void MultiplayerManager_OnFailedToJoinGame()
    {
        string msg = NetworkManager.Singleton.DisconnectReason;
        if (msg == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(msg);
        }
        _closeButton.gameObject.SetActive(true);
    }

    void GameLobby_OnCreateLobbyStarted()
    {
        ShowMessage("Creating lobby...");
        _closeButton.gameObject.SetActive(false);
    }

    void GameLobby_OnCreateLobbyFailed()
    {
        ShowMessage("Failed to create lobby");
        _closeButton.gameObject.SetActive(true);
    }

    void GameLobby_OnJoinStarted()
    {
        ShowMessage("Joining lobby...");
        _closeButton.gameObject.SetActive(false);
    }

    void GameLobby_OnQuickJoinFailed()
    {
        ShowMessage("Could not find a lobby to quick join");
        _closeButton.gameObject.SetActive(true);
    }

    void GameLobby_OnJoinFailed()
    {
        ShowMessage("Failed to join lobby");
        _closeButton.gameObject.SetActive(true);
    }

    void ShowMessage(string message)
    {
        Show();
        _messageText.text = message;
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
