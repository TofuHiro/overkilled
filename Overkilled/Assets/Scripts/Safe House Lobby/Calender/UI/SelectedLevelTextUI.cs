using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedLevelTextUI : MonoBehaviour
{
    TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        LobbyManager.Instance.OnLevelChange += LobbyManager_OnLevelChange;

        SetLevelText("Not Selected");
    }

    void LobbyManager_OnLevelChange(Loader.Level level)
    {
        string levelText = level.ToString();
        levelText = levelText.Replace("_", " ");

        SetLevelText(levelText);
    }

    /// <summary>
    /// Set the current level display
    /// </summary>
    /// <param name="text"></param>
    public void SetLevelText(string text)
    {
        _text.text = "Next Shift: " + text;
    }
}
