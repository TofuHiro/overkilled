using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    void Start()
    {
        GameManager.OnLocalPlayerReady += Hide;

        Show();
    }

    void OnDestroy()
    {
        GameManager.OnLocalPlayerReady -= Hide;
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
