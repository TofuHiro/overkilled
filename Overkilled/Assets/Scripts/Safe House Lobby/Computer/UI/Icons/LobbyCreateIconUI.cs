public class LobbyCreateIconUI : ComputerIconUI
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
