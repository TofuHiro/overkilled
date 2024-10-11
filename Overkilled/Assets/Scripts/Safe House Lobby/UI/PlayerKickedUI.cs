using UnityEngine;
using UnityEngine.UI;

public class PlayerKickedUI : MonoBehaviour
{
    [SerializeField] Button _returnHomeButton;

    void Awake()
    {
        _returnHomeButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
        });
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

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
