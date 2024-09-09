using UnityEngine;
using UnityEngine.UI;

public class NetworkButtons : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;

    void Awake()
    {
        _hostButton.onClick.AddListener(() => { MultiplayerManager.Instance.StartHost(); });
        _clientButton.onClick.AddListener(() => { MultiplayerManager.Instance.StartClient(); });

    }
}
