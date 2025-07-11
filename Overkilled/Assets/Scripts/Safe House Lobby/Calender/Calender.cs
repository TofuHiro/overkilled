using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Calender : MonoBehaviour, IInteractable
{
    [Tooltip("The UI to toggle the display for")]
    [SerializeField] LevelSelectGalleryUI _levelSelectUI;
    [Tooltip("The world space text to display the currently selected level")]
    [SerializeField] TMP_Text _calenderText;

    /// <summary>
    /// If the calender interface is currently open
    /// </summary>
    public bool IsCalenderOpen { get { return _levelSelectUI.isActiveAndEnabled; } }

    void Start()
    {
        LobbyInterface.Instance.OnUICancel += Hide;
        LobbyInterface.Instance.OnMenuToggle += Hide;
        LevelSelectManager.Instance.OnLevelSelectChange += UpdateCalenderText;
    }

    void OnDestroy()
    {
        LevelSelectManager.Instance.OnLevelSelectChange -= UpdateCalenderText;
    }

    public void Interact(PlayerInteraction player)
    {
        _levelSelectUI.Show();

        LobbyInterface.Instance.ToggleInterface(true);
    }

    void UpdateCalenderText(Level level, LevelSelectManager manager)
    {
        _calenderText.text = manager.GetLevelText();
    }

    void Hide()
    {
        if (!IsCalenderOpen)
            return;

        _levelSelectUI.Hide();

        LobbyInterface.Instance.ToggleInterface(false);
    }
}
