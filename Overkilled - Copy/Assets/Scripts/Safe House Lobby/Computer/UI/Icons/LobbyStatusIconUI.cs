using UnityEngine;

public class LobbyStatusIconUI : ComputerIconUI
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
