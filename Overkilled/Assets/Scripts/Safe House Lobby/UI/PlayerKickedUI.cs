using UnityEngine;
using UnityEngine.UI;

public class PlayerKickedUI : MonoBehaviour
{
    [SerializeField] Button _returnHomeButton;

    void Awake()
    {
        _returnHomeButton.onClick.AddListener(Quit);
    }

    void Start()
    {
        MultiplayerManager.Instance.OnLocalDisconnect += Show;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.Instance.OnLocalDisconnect -= Show;
    }

    async void Quit()
    {
        await MultiplayerManager.Instance.LeaveMultiplayer();

        LobbyManager.Instance.ReloadLobby();
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
