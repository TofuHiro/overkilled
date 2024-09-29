using SurvivalGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
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
        //When host quits, clients are kicked = show message.
        //If client quits, they return to menu 
        MultiplayerManager.Instance.OnLocalDisconnect += Show;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.Instance.OnLocalDisconnect -= Show;
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
