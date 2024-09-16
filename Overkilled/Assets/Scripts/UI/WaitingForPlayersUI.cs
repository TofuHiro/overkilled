using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.OnLocalPlayerReady += Show;
        GameManager.Instance.OnGameStateChange += HideOnCountdown;

        Hide();
    }

    void HideOnCountdown()
    {
        if (GameManager.Instance.IsStarting)
        {
            Hide();
        }
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
