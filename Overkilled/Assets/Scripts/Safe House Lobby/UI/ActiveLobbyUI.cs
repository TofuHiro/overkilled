using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActiveLobbyUI : MonoBehaviour
{
    [Tooltip("Text to display the name of the lobby")]
    [SerializeField] TMP_Text _lobbyNameText;
    [Tooltip("Text to display the number of players in the lobby")]
    [SerializeField] TMP_Text _lobbyPlayerCountText;

    Button _button;
    Lobby _lobby;

    void Awake()
    {
        _button = GetComponent<Button>();
        
        _button.onClick.AddListener(() =>
        {
            _button.enabled = false;

            GameLobby.Instance.JoinLobbyWithId(_lobby.Id);
        }); 
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
    }

    void GameLobby_OnLobbySuccess()
    {
        _button.enabled = true;
    }

    /// <summary>
    /// Assign a lobby to display its details
    /// </summary>
    /// <param name="lobby"></param>
    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyNameText.text = _lobby.Name;
        _lobbyPlayerCountText.text = _lobby.Players.Count.ToString() + "/" + _lobby.MaxPlayers.ToString();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
