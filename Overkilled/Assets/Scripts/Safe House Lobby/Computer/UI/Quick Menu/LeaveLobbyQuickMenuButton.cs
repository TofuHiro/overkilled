using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveLobbyQuickMenuButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LeaveLobby);    
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += Show;
        GameLobby.Instance.OnJoinSuccess += Show;

        if (GameLobby.Instance.InLobby)
            Show();
        else
            Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= Show;
        GameLobby.Instance.OnJoinSuccess -= Show;
    }

    async void LeaveLobby()
    {
        try
        {
            await MultiplayerManager.Instance.LeaveMultiplayer();

            Loader.LoadScene(Loader.Scene.SafeHouseScene);
        }
        catch (Exception e)
        {
            Debug.LogError("Error trying to leave multiplayer lobby" + "\n" + e);
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
