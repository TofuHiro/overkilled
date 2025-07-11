using SurvivalGame;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PostGameUI : MonoBehaviour
{
    [Tooltip("Text displaying the final grade")]
    [SerializeField] TMP_Text _gradeText;

    [Header("Host")]
    [Tooltip("Gameobject parenting all host UI")]
    [SerializeField] GameObject _hostOptions;
    [Tooltip("Button to replay the current level")]
    [SerializeField] Button _replayButton;
    [Tooltip("Button to return to lobby with teammates")]
    [SerializeField] Button _returnToLobbyWithTeamButton;

    [Header("Client")]
    [Tooltip("Gameobject parenting all client UI")]
    [SerializeField] GameObject _clientOptions;
    [Tooltip("Text displaying waiting for host")]
    [SerializeField] TMP_Text _waitingForHostText;
    [Tooltip("Button to return to lobby, leaving the current team")]
    [SerializeField] Button _returnToLobbyClientButton;

    void Awake()
    {
        _replayButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RestartGame();
        });

        _returnToLobbyWithTeamButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToLobby();
        });

        _returnToLobbyClientButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LeaveTeamToLobby();
        });
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
        
        Hide();
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.GameEnded)
        {
            _gradeText.text = GetGradeText(GameManager.Instance.GetGrade());
            Show();
        }
    }

    //Temp
    string GetGradeText(Grade grade)
    {
        switch (grade)
        {
            case Grade.ThreeStars:
                return "Three Stars";
            case Grade.TwoStars:
                return "Two Stars";
            case Grade.OneStar:
                return "One Stars";
            default: //Grade.NoStars:
                return "No Stars";
        }
    }

    void Show()
    {
        gameObject.SetActive(true);

        _hostOptions.SetActive(NetworkManager.Singleton.IsServer);
        _clientOptions.SetActive(!NetworkManager.Singleton.IsServer);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
