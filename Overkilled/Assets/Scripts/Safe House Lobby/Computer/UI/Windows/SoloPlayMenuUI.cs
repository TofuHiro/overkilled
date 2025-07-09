using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloPlayMenuUI : MonoBehaviour
{
    [SerializeField] TMP_Text _levelText;
    [SerializeField] Button _playButton;

    void Awake()
    {
        _playButton.onClick.AddListener(StartGame);    
    }

    void Start()
    {
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;

        Hide();    
    }

    void OnDestroy()
    {
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
       _levelText.text = manager.GetLevelText();
    }

    void StartGame()
    {
        LobbyManager.Instance.StartGame(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ResetMenu();
        gameObject.SetActive(false);
    }

    void ResetMenu()
    {
        LevelSelectManager.Instance.SetLevel(Level.None);
    }
}
