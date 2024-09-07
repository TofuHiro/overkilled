using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPauseScreenUI : MonoBehaviour
{
    void Start()
    {
        GameManager.OnMultiplayerGamePause += Show;
        GameManager.OnMultiplayerGameUnpause += Hide;

        Hide();
    }

    void OnEnable()
    {
        GameManager.OnMultiplayerGamePause += Show;
        GameManager.OnMultiplayerGameUnpause += Hide;
    }

    void OnDisable()
    {
        GameManager.OnMultiplayerGamePause -= Show;
        GameManager.OnMultiplayerGameUnpause -= Hide;
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
