using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Tooltip("Button to toggle player ready state")]
    [SerializeField] Button _readyButton;
    [Tooltip("Button to try to start the game")]
    [SerializeField] Button _startButton;
    [Tooltip("Button to return to the main menu")]
    [SerializeField] Button _mainMenuButton;
    [Tooltip("Text to display the lobby code")]
    [SerializeField] TMP_Text _lobbyCodeText;

    void Awake()
    {
        _readyButton.onClick.AddListener(() => 
        {
            CharacterSelectReady.Instance.TogglePlayerReady();
        });

        _startButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.StartGame();
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
    }

    void Start()
    {
        //Server/host only
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        Lobby lobby = GameLobby.Instance.GetLobby();
        _lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
