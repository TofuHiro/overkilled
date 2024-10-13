using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisconnectMessageUI : MonoBehaviour
{
    [SerializeField] TMP_Text _disconnectReasonText;
    [SerializeField] float _displayTime = 3f;

    float _timer = 0f;
    bool _displaying = false;

    void Start()
    {
        MultiplayerManager.Instance.OnLobbyReloadAfterDisconnect += UpdateMessage;

        Hide();
    }

    void OnDestroy()
    {
        MultiplayerManager.Instance.OnLobbyReloadAfterDisconnect -= UpdateMessage;
    }

    void Update()
    {
        if (!_displaying)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _displayTime)
            Hide();
    }

    void UpdateMessage(string reason)
    {
        _disconnectReasonText.text = reason;

        Show();
    }

    void Show()
    {
        _displaying = true;
        gameObject.SetActive(true);
    }

    void Hide()
    {
        _timer = 0f;
        _displaying = false;
        gameObject.SetActive(false);
    }
}
