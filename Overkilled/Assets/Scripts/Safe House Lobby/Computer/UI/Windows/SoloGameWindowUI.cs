using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloGameWindowUI : ComputerWindowUI
{
    [SerializeField] Button _startButton;
    [SerializeField] TMP_Text _levelText;

    protected override void Awake()
    {
        base.Awake();

        _startButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.StartSoloGame();
        });  
    }

    void Start()
    {
        LobbyManager.Instance.OnLevelChange += LobbyManager_OnLevelChange;
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
    }

    void GameLobby_OnLobbySuccess()
    {
        Hide();
    }

    void LobbyManager_OnLevelChange(Loader.Level level)
    {
        string levelText = level.ToString();
        levelText = levelText.Replace("_", " ");
        _levelText.text = "Next Shift: " + levelText.ToString();
    }
}
