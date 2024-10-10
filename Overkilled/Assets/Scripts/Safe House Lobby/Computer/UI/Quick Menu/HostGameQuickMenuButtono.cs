using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostGameQuickMenuButtono : QuickMenuButtonUI
{
    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += Hide;
        GameLobby.Instance.OnJoinSuccess += Hide;

        if (GameLobby.Instance.InLobby)
            Hide();
        else
            Show();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= Hide;
        GameLobby.Instance.OnJoinSuccess -= Hide;
    }
}
