using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [Tooltip("Button to exit the create lobby UI")]
    [SerializeField] Button _exitButton;
    [Tooltip("Button to confirm creation of a lobby")]
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
            string lobbyName = _lobbyNameInputField.text;
            if (string.IsNullOrWhiteSpace(lobbyName))
                lobbyName = "Lobby Name " + Random.Range(0, 1000);
            GameLobby.Instance.CreateLobby(lobbyName, _privateLobbyToggle.isOn);
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
