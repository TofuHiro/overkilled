using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    void Start()
    {
        GameManager.OnLocalPlayerReady += Show;
        GameManager.OnGameStateChange += HideOnCountdown;

        Hide();
    }

    void OnEnable()
    {
        GameManager.OnLocalPlayerReady += Show;
        GameManager.OnGameStateChange += HideOnCountdown;
    }

    void OnDisable()
    {
        GameManager.OnLocalPlayerReady -= Show;
        GameManager.OnGameStateChange -= HideOnCountdown;
    }

    void HideOnCountdown()
    {
        if (GameManager.Instance.GameStarting)
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
