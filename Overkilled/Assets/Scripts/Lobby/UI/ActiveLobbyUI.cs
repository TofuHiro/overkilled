using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ActiveLobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text _lobbyNameText;
    [SerializeField] TMP_Text _lobbyPlayerCountText;

    Lobby _lobby;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinLobbyWithId(_lobby.Id);
        }); 
    }

    void Start()
    {
        Hide();
    }

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
