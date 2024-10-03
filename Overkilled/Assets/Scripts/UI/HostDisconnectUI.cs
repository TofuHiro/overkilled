using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [Tooltip("The return to lobby button on the Pause Screen UI")]
    [SerializeField] Button _toLobbyButton;
    [Tooltip("The return to menu on the Pause Screen UI")]
    [SerializeField] Button _toMenuButton;

    //Return to lobby?

    void Awake()
    {
        _toLobbyButton.onClick.AddListener(ToLobby);

        _toMenuButton.onClick.AddListener(ToMenu);
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

    async void ToLobby()
    {
        await MultiplayerManager.Instance.LeaveMultiplayer();

        SceneManager.LoadScene(Loader.Scene.SafeHouseScene.ToString());
    }

    async void ToMenu()
    {
        await MultiplayerManager.Instance.LeaveMultiplayer();

        SceneManager.LoadScene(Loader.Scene.MainMenuScene.ToString());
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
