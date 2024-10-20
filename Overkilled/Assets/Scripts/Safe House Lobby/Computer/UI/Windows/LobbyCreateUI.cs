using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : ComputerWindowUI
{
    [Header("Lobby Creation")]
    [Tooltip("Button to confirm creation of a lobby")]
    [SerializeField] Button _createLobbyButton;

    [Header("Creation Settings")]
    [Tooltip("Input field to enter a name for the lobby")]
    [SerializeField] TMP_InputField _lobbyNameInputField;
    [Tooltip("Toggle box to set whether the lobby is private or public")]
    [SerializeField] Toggle _privateLobbyToggle;

    [Header("Max Player Toggles")]
    [Tooltip("Button to set 2 players")]
    [SerializeField] Button _twoPlayersButton;
    [Tooltip("Check mark to display 2 max players option as active")]
    [SerializeField] Image _twoPlayerCheckMark;
    [Tooltip("Button to set 3 players")]
    [SerializeField] Button _threePlayersButton;
    [Tooltip("Check mark to display 3 max players option as active")]
    [SerializeField] Image _threePlayerCheckMark;
    [Tooltip("Button to set 4 players")]
    [SerializeField] Button _fourPlayersButton;
    [Tooltip("Check mark to display 4 max players option as active")]
    [SerializeField] Image _fourPlayerCheckMark;

    int _maxPlayers;

    protected override void Awake()
    {
        base.Awake();

        _createLobbyButton.onClick.AddListener(() =>
        {
            string lobbyName = _lobbyNameInputField.text;
            if (string.IsNullOrWhiteSpace(lobbyName))
                lobbyName = "Lobby Name " + Random.Range(0, 1000);

            GameLobby.Instance.CreateLobby(lobbyName, _privateLobbyToggle.isOn, _maxPlayers);
        });

        _twoPlayersButton.onClick.AddListener(() =>
        {
            SetMaxPlayers(2);
        });

        _threePlayersButton.onClick.AddListener(() =>
        {
            SetMaxPlayers(3);
        });

        _fourPlayersButton.onClick.AddListener(() =>
        {
            SetMaxPlayers(4);
        });

        SetMaxPlayers(2);
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        Hide();
    }
   
    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
    }

    void GameLobby_OnCreateLobbyStarted()
    {
        _createLobbyButton.enabled = false;
    }

    void GameLobby_OnCreateLobbyFailed()
    {
        _createLobbyButton.enabled = true;
    } 

    void GameLobby_OnJoinStarted()
    {
        _createLobbyButton.enabled = false;
    }

    void GameLobby_OnJoinFailed()
    {
        _createLobbyButton.enabled = true;
    }

    void GameLobby_OnLobbySuccess()
    {
        Hide();

        _createLobbyButton.enabled = true;
    }

    public override void Hide()
    {
        ResetCreation();
        base.Hide();
    }

    void ResetCreation()
    {
        _lobbyNameInputField.text = "";
        _privateLobbyToggle.isOn = false;
    }

    void SetMaxPlayers(int maxPlayers)
    {
        _maxPlayers = maxPlayers;

        _twoPlayerCheckMark.enabled = maxPlayers == 2;
        _threePlayerCheckMark.enabled = maxPlayers == 3;
        _fourPlayerCheckMark.enabled = maxPlayers == 4;
    }
}
