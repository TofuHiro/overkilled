using UnityEngine;

public class LobbyJoinIconUI : ComputerIconUI
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
