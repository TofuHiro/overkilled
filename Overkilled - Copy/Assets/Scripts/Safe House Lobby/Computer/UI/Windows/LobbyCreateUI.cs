using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [Header("Lobby Creation")]
    [Tooltip("Button to confirm creation of a lobby")]
    [SerializeField] Button _createLobbyButton;
    [SerializeField] TMP_Text _title;

    [Header("Naming")]
    [Tooltip("To de/activate depending on lobby privacy")]
    [SerializeField] GameObject _lobbyNameObject;
    [Tooltip("Input field to enter a name for the lobby")]
    [SerializeField] TMP_InputField _lobbyNameInputField;

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

    string CREATING_PRIVATE = "Creating Private Lobby";
    string CREATING_PUBLIC = "Creating Public Lobby";

    int _maxPlayers;
    bool _isPrivate;

    void Awake()
    {
        _createLobbyButton.onClick.AddListener(() =>
        {
            string lobbyName = _lobbyNameInputField.text;
            if (string.IsNullOrWhiteSpace(lobbyName))
                lobbyName = "Lobby Name " + Random.Range(0, 1000);

            GameLobby.Instance.CreateLobby(lobbyName, _isPrivate, _maxPlayers);
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

        ResetCreation();
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
        _createLobbyButton.enabled = true;

        Hide();
    }

    public void ResetCreation()
    {
        _lobbyNameInputField.text = "";
        SetMaxPlayers(4);
    }

    public void SetPrivate(bool isPrivate)
    {
        _isPrivate = isPrivate;
        _title.text = isPrivate ? CREATING_PRIVATE : CREATING_PUBLIC;
        _lobbyNameObject.SetActive(!isPrivate);
    }

    void SetMaxPlayers(int maxPlayers)
    {
        _maxPlayers = maxPlayers;

        _twoPlayerCheckMark.enabled = maxPlayers == 2;
        _threePlayerCheckMark.enabled = maxPlayers == 3;
        _fourPlayerCheckMark.enabled = maxPlayers == 4;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _createLobbyButton.enabled = true;
    }

    public void Hide()
    {
        ResetCreation();
        gameObject.SetActive(false);
    }
}
