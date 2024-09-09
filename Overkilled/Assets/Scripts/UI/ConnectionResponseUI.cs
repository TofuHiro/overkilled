using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField] TMP_Text _messageText;
    [SerializeField] Button _closeButton;

    void Awake()
    {
        _closeButton.onClick.AddListener(Hide);
    }

    void Start()
    {
        MultiplayerManager.OnFailedToJoinGame += Show;
        MultiplayerManager.OnFailedToJoinGame += UpdateMessage;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.OnFailedToJoinGame -= Show;
        MultiplayerManager.OnFailedToJoinGame -= UpdateMessage;
    }

    void UpdateMessage()
    {
        _messageText.text = NetworkManager.Singleton.DisconnectReason;
        
        if (_messageText.text == "")
            _messageText.text = "Failed to connect";
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
