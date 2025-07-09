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
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;

        SetLevelText("Not Selected");
    }

    void OnDestroy()
    {
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        SetLevelText(manager.GetLevelText());
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
