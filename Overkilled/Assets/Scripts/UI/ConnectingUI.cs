using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    void Start()
    {
        MultiplayerManager.OnTryingToJoinGame += Show;
        MultiplayerManager.OnFailedToJoinGame += Hide;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.OnTryingToJoinGame -= Show;
        MultiplayerManager.OnFailedToJoinGame -= Hide;
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
