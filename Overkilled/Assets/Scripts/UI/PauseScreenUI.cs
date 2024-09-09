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
    [Tooltip("The quit button on the Pause Screen UI")]
    [SerializeField] Button _quitButton;

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

        _quitButton.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.Shutdown();
            //Load menu
        });
    }

    void Start()
    {
        GameManager.OnLocalGamePause += Show;
        GameManager.OnLocalGameUnpause += Hide;

        Hide();    
    }

    void OnDestroy()
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
