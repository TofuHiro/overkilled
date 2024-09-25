using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStatusIconUI : ComputerIconUI
{
    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += Show;
        GameLobby.Instance.OnJoinSuccess += Show;

        Hide();
    }
}
