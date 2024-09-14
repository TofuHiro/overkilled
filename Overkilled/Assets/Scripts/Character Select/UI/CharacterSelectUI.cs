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
    [SerializeField] Button _mainMenuButton;
    [SerializeField] TMP_Text _lobbyCodeText;

    void Awake()
    {
        _readyButton.onClick.AddListener(() => 
        {
            CharacterSelectReady.Instance.SetPlayerReady();
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
        Lobby lobby = GameLobby.Instance.GetLobby();
        _lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
