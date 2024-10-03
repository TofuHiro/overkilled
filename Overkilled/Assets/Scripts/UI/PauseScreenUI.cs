using SurvivalGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenUI : MonoBehaviour
{
    [Tooltip("The resume button on the Pause Screen UI")]
    [SerializeField] Button _resumeButton;
    [Tooltip("The restart button on the Pause Screen UI")]
    [SerializeField] Button _restartButton;
    [Tooltip("The return to lobby button on the Pause Screen UI")]
    [SerializeField] Button _toLobbyButton;
    [Tooltip("The return to menu on the Pause Screen UI")]
    [SerializeField] Button _toMenuButton;

    void Awake()
    {
        _resumeButton.onClick.AddListener(() => 
        { 
            GameManager.Instance.TogglePauseGame(); 
        });

        _restartButton.onClick.AddListener(() => 
        {
            //GameManager restart
        });

        _toLobbyButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Loader.LoadSceneNetwork(Loader.Scene.SafeHouseScene);
            }
            else
            {
                ToLobbyLocal();
            }
        });

        _toMenuButton.onClick.AddListener(ToMenu);
    }

    void Start()
    {
        GameManager.Instance.OnLocalGamePause += Show;
        GameManager.Instance.OnLocalGameUnpause += Hide;

        Hide();    
    }

    async void ToLobbyLocal()
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

        _restartButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
