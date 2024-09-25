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

        _text.text = "Next Shift: Not Selected";
    }

    void LobbyManager_OnLevelChange(Loader.Level level)
    {
        string levelText = level.ToString();
        levelText = levelText.Replace("_", " ");

        _text.text = "Next Shift: " + levelText;
    }
}
