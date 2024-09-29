using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoinUI : ComputerWindowUI
{
    [Header("Join Game")]
    //[SerializeField] Button _quickJoinButton;
    [Tooltip("Button to join with an entered code")]
    [SerializeField] Button _joinCodeButton;
    [Tooltip("The input field to enter a lobby code to join")]
    [SerializeField] TMP_InputField _joinCodeInputField;
    [Header("Lobby")]
    [Tooltip("The container holding all the lobby templates/active lobbies")]
    [SerializeField] Transform _lobbyContainer;
    [Tooltip("The template to use for displaying active lobbies")]
    [SerializeField] Transform _lobbyTemplate;

    //Used to track and reuse templates
    List<ActiveLobbyUI> _instantiatedLobbyTemplates;

    protected override void Awake()
    {
        base.Awake();

        /*_quickJoinButton.onClick.AddListener(() => 
        {
            GameLobby.Instance.QuickJoin();
        });*/

        _joinCodeButton.onClick.AddListener(() =>
        {
            _joinCodeButton.enabled = false;

            GameLobby.Instance.JoinLobbyWithCode(_joinCodeInputField.text);
        });

        _instantiatedLobbyTemplates = new List<ActiveLobbyUI>();
    }

    void Start()
    {
        GameLobby.Instance.OnLobbyListChanged += UpdateLobbyList;
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        UpdateLobbyList(new List<Lobby>());
        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= UpdateLobbyList;
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
    }

    void GameLobby_OnLobbySuccess()
    {
        Hide();

        _joinCodeButton.enabled = true;
    }

    void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (ActiveLobbyUI lobby in _instantiatedLobbyTemplates)
        {
            lobby.Hide();
        }

        int currentInstantiatedLobbyUICount = _instantiatedLobbyTemplates.Count;
        for (int i = 0; i < lobbyList.Count; i++)
        {
            if (i > currentInstantiatedLobbyUICount - 1)
            {
                ActiveLobbyUI newLobby = Instantiate(_lobbyTemplate, _lobbyContainer).GetComponent<ActiveLobbyUI>();
                newLobby.Show();
                newLobby.SetLobby(lobbyList[i]);
                _instantiatedLobbyTemplates.Add(newLobby);
            }
            else
            {
                _instantiatedLobbyTemplates[i].Show();
                _instantiatedLobbyTemplates[i].SetLobby(lobbyList[i]);
            }
        }
    }
}
