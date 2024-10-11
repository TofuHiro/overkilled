using SurvivalGame;
using UnityEngine;
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
        _toLobbyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LeaveTeamToLobby();
        });

        _toMenuButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToMenu();
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
