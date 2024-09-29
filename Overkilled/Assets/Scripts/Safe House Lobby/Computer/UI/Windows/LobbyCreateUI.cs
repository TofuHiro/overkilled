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

    protected override void Awake()
    {
        base.Awake();

        _createLobbyButton.onClick.AddListener(() =>
        {
            _createLobbyButton.enabled = false;

            string lobbyName = _lobbyNameInputField.text;
            if (string.IsNullOrWhiteSpace(lobbyName))
                lobbyName = "Lobby Name " + Random.Range(0, 1000);
            GameLobby.Instance.CreateLobby(lobbyName, _privateLobbyToggle.isOn);
        });
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess; ;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        Hide();
    }

    void GameLobby_OnLobbySuccess()
    {
        Hide();

        _createLobbyButton.enabled = true;
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
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
}
