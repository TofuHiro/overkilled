using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public static Computer Instance { get; private set; }

    [Tooltip("The computer UI to display")]
    [SerializeField] ComputerUI _computerUI;

    /// <summary>
    /// If the computer currently has any windows open
    /// </summary>
    public bool HasWindowOpen { get { return _openedWindows.Count > 0; } }

    /// <summary>
    /// If the computer interface is open
    /// </summary>
    public bool IsComputerOpen { get { return _computerUI.isActiveAndEnabled; } }

    List<ComputerWindowUI> _openedWindows;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Computer found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        _openedWindows = new List<ComputerWindowUI>();
    }

    void Start()
    {
        LobbyInterface.Instance.OnUICancel += LobbyInterface_OnUICancel;
        LobbyInterface.Instance.OnMenuToggle += LobbyInterface_OnMenuToggle;
    }

    public void Interact(PlayerInteraction player)
    {
        Show();
    }

    void Show()
    {
        _computerUI.Show();
        LobbyInterface.Instance.ToggleInterface(true);
    }

    void LobbyInterface_OnMenuToggle()
    {
        if (!IsComputerOpen && !LobbyInterface.Instance.IsInterfaceOpen)
            Show();
        else if (IsComputerOpen)
            Hide();
    }

    void LobbyInterface_OnUICancel()
    {
        if (!IsComputerOpen)
            return;

        if (HasWindowOpen)
            Back();
        else
            Hide();
    }

    void Hide()
    {
        _computerUI.Hide();
        LobbyInterface.Instance.ToggleInterface(false);
    }

    public void OpenWindow(ComputerWindowUI window)
    {
        window.Show();
        //Place on top of all other windows
        window.transform.SetAsLastSibling();

        //Readding to add to end of list -> close top window will close this window
        if (_openedWindows.Contains(window))
            _openedWindows.Remove(window);

        _openedWindows.Add(window);
    }

    public void CloseWindow(ComputerWindowUI window)
    {
        window.Hide();
        _openedWindows.Remove(window);
    }

    void Back()
    {
        ComputerWindowUI window = _openedWindows[_openedWindows.Count - 1];
        window.Back();
    }
}
