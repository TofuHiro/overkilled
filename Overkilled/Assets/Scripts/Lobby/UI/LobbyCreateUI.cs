using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] Button _exitButton;
    [SerializeField] Button _createLobbyButton;
    [Header("Creation Settings")]
    [Tooltip("Input field to enter a name for the lobby")]
    [SerializeField] TMP_InputField _lobbyNameInputField;
    [Tooltip("Toggle box to set whether the lobby is private or public")]
    [SerializeField] Toggle _privateLobbyToggle;

    void Awake()
    {
        _createLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(_lobbyNameInputField.text, _privateLobbyToggle.isOn);
        });
        _exitButton.onClick.AddListener(() =>
        {
            ResetCreation();
            Hide();
        });
    }

    void Start()
    {
        Hide();    
    }

    void ResetCreation()
    {
        _lobbyNameInputField.text = "";
        _privateLobbyToggle.isOn = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
