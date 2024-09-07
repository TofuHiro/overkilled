using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenUI : MonoBehaviour
{
    void Start()
    {
        GameManager.OnLocalGamePause += Show;
        GameManager.OnLocalGameUnpause += Hide;

        Hide();    
    }

    void OnEnable()
    {
        GameManager.OnLocalGamePause += Show;
        GameManager.OnLocalGameUnpause += Hide;
    }

    void OnDisable()
    {
        GameManager.OnLocalGamePause -= Show;
        GameManager.OnLocalGameUnpause -= Hide;
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
