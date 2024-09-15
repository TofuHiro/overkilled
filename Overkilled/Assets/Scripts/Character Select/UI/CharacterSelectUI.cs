using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] Button _readyButton;
    [SerializeField] Button _startButton;
    [SerializeField] Button _mainMenuButton;
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
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        Lobby lobby = GameLobby.Instance.GetLobby();
        _lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
