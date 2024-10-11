using UnityEngine;

public class LobbyStatusIconUI : MonoBehaviour
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

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
