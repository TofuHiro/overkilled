using TMPro;
using UnityEngine;

public class CalenderWindowUI : ComputerWindowUI
{
    [SerializeField] TMP_Text _selectedLevel;

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        _selectedLevel.text = manager.GetLevelText();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void GameLobby_OnLobbySuccess()
    {
        Hide();
    }
}
