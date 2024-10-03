using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PostGameUI : MonoBehaviour
{

    [SerializeField] Button _replayButton;
    [SerializeField] Button _returnToLobbyButton;
    [Tooltip("Text displaying the final grade")]
    [SerializeField] TMP_Text _gradeText;
    [Tooltip("Text displaying waiting for host")]
    [SerializeField] TMP_Text _waitingForHostText;

    void Awake()
    {
        _returnToLobbyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReturnToLobby();
        });

        _replayButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RestartGame();
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
            _gradeText.text = GetGradeText(GameManager.Instance.LevelGrade);
            Show();
        }
    }

    //Temp
    string GetGradeText(Grade grade)
    {
        switch (grade)
        {
            case Grade.FiveStars:
                return "Five Stars";
            case Grade.FourStars:
                return "Four Stars";
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

        _returnToLobbyButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        _replayButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        _waitingForHostText.gameObject.SetActive(!NetworkManager.Singleton.IsServer);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
