using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnyLevelCalenderSelect : MonoBehaviour, IStartInvoke
{
    [Tooltip("The UI element to display that this level is currently selected")]
    [SerializeField] GameObject _selectedOverlay;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            LevelSelectManager.Instance.SetLevel(Level.None);
        });

        LobbyJoinUI.OnJoinLobbyToggle += LobbyJoinUI_OnJoinLobbyToggle;
    }

    void Start()
    {
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;
    }

    void OnDestroy()
    {
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
        LobbyJoinUI.OnJoinLobbyToggle -= LobbyJoinUI_OnJoinLobbyToggle;
    }

    public void InvokeStart()
    {
        _selectedOverlay.SetActive(false);
        gameObject.SetActive(false);
    }

    void LevelSelectManager_OnLevelSelectChange(Level level)
    {
        if (level == Level.None)
        {
            _selectedOverlay.SetActive(true);
        }
        else
        {
            _selectedOverlay.SetActive(false);
        }
    }

    void LobbyJoinUI_OnJoinLobbyToggle(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
