using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button _mainMenuButton;
    [Header("Create Game")]
    [SerializeField] Button _createGameButton;
    [Tooltip("Lobby Create UI instance to show when selecting create game")]
    [SerializeField] LobbyCreateUI _createGameUI;
    [Header("Join Game")]
    [SerializeField] Button _quickJoinButton;
    [SerializeField] Button _joinCodeButton;
    [Tooltip("The input field to enter a lobby code to join")]
    [SerializeField] TMP_InputField _joinCodeInputField;
    [Header("Lobby")]
    [SerializeField] Transform _lobbyContainer;
    [SerializeField] Transform _lobbyTemplate;
    [SerializeField] Button _refreshListButton;

    List<ActiveLobbyUI> _instantiatedLobbyTemplates;

    void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });

        _createGameButton.onClick.AddListener(() =>
        {
            _createGameUI.Show();
        });

        _quickJoinButton.onClick.AddListener(() => 
        {
            GameLobby.Instance.QuickJoin();
        });

        _joinCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinLobbyWithCode(_joinCodeInputField.text);
        });

        _refreshListButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.ListLobbies();
        });

        _instantiatedLobbyTemplates = new List<ActiveLobbyUI>();
    }

    void Start()
    {
        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    void GameLobby_OnLobbyListChanged(List<Lobby> lobbyList)
    {
        UpdateLobbyList(lobbyList);
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
