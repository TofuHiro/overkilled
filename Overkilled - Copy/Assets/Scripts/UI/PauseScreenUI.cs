using SurvivalGame;
using Unity.Netcode;
using UnityEngine;
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
            GameManager.Instance.RestartGame();
        });

        _toLobbyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToLobby();
        });

        _toMenuButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToMenu();
        });
    }

    void Start()
    {
        GameManager.Instance.OnLocalGamePause += Show;
        GameManager.Instance.OnLocalGameUnpause += Hide;

        Hide();
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
