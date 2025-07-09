using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbySearchLevelSelectButton : MonoBehaviour
{
    [Tooltip("Text on the button to open up calender to pick level")]
    [SerializeField] TMP_Text _levelSelectText;

    void Start()
    {
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;    
    }

    void OnDestroy()
    {
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        if (level == Level.None)
        {
            _levelSelectText.text = "Any";
        }
        else
        {
            _levelSelectText.text = manager.GetLevelText();
        }
    }
}
