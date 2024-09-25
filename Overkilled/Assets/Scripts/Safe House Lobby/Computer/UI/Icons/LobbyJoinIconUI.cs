using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyJoinIconUI : ComputerIconUI
{
    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += Hide;
        GameLobby.Instance.OnJoinSuccess += Hide;
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= Hide;
        GameLobby.Instance.OnJoinSuccess -= Hide;
    }
}
