using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerKickedUI : MonoBehaviour
{
    [Tooltip("The quit button on the Host Disconnect UI")]
    [SerializeField] Button _quitButton;

    void Awake()
    {
        _quitButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(Loader.Scene.MainMenuScene.ToString());
        });
    }

    void Start()
    {
        MultiplayerManager.OnDisconnect += Show;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.OnDisconnect -= Show;
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
