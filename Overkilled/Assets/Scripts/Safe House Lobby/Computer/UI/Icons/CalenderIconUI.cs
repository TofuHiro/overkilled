using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CalenderIconUI : MonoBehaviour
{
    void Start()
    {
        GameLobby.Instance.OnJoinSuccess += Hide;

        if (GameLobby.Instance.InLobby && !NetworkManager.Singleton.IsServer)
            Hide();
        else
            Show();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnJoinSuccess -= Hide;
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
