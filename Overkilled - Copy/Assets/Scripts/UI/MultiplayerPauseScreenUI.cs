using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerPauseScreenUI : MonoBehaviour
{
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            GameManager.Instance.OnMultiplayerGamePause += Show;
            GameManager.Instance.OnMultiplayerGameUnpause += Hide;
        }

        Hide();
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
