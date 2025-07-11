using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStatusUI : ComputerWindowUI
{
    [Tooltip("Button to leave the lobby")]
    [SerializeField] Button _leaveButton;
    [Tooltip("Button to toggle player ready state")]
    [SerializeField] Button _readyButton;
    [Tooltip("Button to try to start the game")]
    [SerializeField] Button _startButton;
    [Tooltip("Text to display the lobby name")]
    [SerializeField] TMP_Text _lobbyName;
    [Tooltip("Text to display the lobby code")]
    [SerializeField] TMP_Text _lobbyCodeText;

    [SerializeField] TMP_Text _selectedLevel;

    protected override void Awake()
    {
        base.Awake();

        _leaveButton.onClick.AddListener(() =>
        {
            _leaveButton.enabled = false;
            LobbyManager.Instance.LeaveLobby();
        });

        _readyButton.onClick.AddListener(() =>
        {
            PlayerReadyManager.Instance.TogglePlayerReady();
        });

        _startButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.StartGame();
        });
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += UpdateLobby;
        GameLobby.Instance.OnJoinSuccess += UpdateLobby;
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;

        if (GameLobby.Instance.InLobby)
            UpdateLobby();

        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= UpdateLobby;
        GameLobby.Instance.OnJoinSuccess -= UpdateLobby;
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        _selectedLevel.text = manager.GetLevelText();
    }

    void UpdateLobby()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();
        _lobbyName.text = lobby.Name;
        _lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;

        //Server/host only
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
    }
}
