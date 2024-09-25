using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Calender : MonoBehaviour, IInteractable
{
    [SerializeField] LevelSelectUI _levelSelectUI;
    [SerializeField] TMP_Text _calenderText;

    public bool IsCalenderOpen { get { return _levelSelectUI.isActiveAndEnabled; } }

    void Start()
    {
        LobbyInterface.Instance.OnUICancel += Close;
        LobbyManager.Instance.OnLevelChange += UpdateCalenderText;
    }

    public void Interact(PlayerInteraction player)
    {
        _levelSelectUI.Show();

        LobbyInterface.Instance.ToggleInterface(true);
    }

    void UpdateCalenderText(Loader.Level level)
    {
        string text = level.ToString();
        text = text.ToUpper();
        text = text.Replace("_", " ");

        _calenderText.text = text;
    }

    void Close()
    {
        if (!IsCalenderOpen)
            return;

        _levelSelectUI.Hide();

        LobbyInterface.Instance.ToggleInterface(false);
    }
}
