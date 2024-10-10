using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStatusQuickMenuButton : QuickMenuButtonUI
{
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
}
