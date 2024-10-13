using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyMessageUI : ComputerWindowUI
{
    [Header("Message")]
    [Tooltip("Text to display lobby messages")]
    [SerializeField] TMP_Text _messageText;

    void Start()
    {
        GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnCreateLobbyCompleted;
        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnJoinSuccess;

        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnCreateLobbyCompleted;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnJoinSuccess;
    }

    void GameLobby_OnCreateLobbyStarted()
    {
        ShowMessage("Creating lobby...");
    }

    void GameLobby_OnCreateLobbyCompleted()
    {
        ShowMessage("Lobby Created");
    }

    void GameLobby_OnCreateLobbyFailed()
    {
        ShowMessage("Failed to create lobby");
    }

    void GameLobby_OnJoinStarted()
    {
        ShowMessage("Joining lobby...");
    }

    void GameLobby_OnQuickJoinFailed()
    {
        ShowMessage("Could not find a lobby to quick join");
    }

    void GameLobby_OnJoinFailed()
    {
        ShowMessage("Failed to join lobby");
    }

    void GameLobby_OnJoinSuccess()
    {
        ShowMessage("Lobby Joined");
    }

    void ShowMessage(string message)
    {
        Show();
        _messageText.text = message;

        Computer.Instance.AddWindow(this);
    }
}
