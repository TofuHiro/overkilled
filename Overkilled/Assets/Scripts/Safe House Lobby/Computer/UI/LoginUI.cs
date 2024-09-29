using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Tooltip("The button to submit the player's name")]
    [SerializeField] Button _loginButton;
    [Tooltip("The input field to enter the player's name")]
    [SerializeField] TMP_InputField _playerNameInputField;

    void Awake()
    {
        _loginButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            MultiplayerManager.Instance.SetPlayerName(newText);
        });
    }

    void Start()
    {
        _playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();

        gameObject.SetActive(true);
    }
}
