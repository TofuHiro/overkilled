using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] Button _readyButton;
    [SerializeField] Button _mainMenuButton;

    void Awake()
    {
        _readyButton.onClick.AddListener(() => 
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
    }
}
