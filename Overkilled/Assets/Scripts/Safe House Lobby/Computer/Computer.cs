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
        LobbyInterface.Instance.OnUICancel += Close;
        LobbyInterface.Instance.OnMenuToggle += LobbyInterface_OnMenuToggle;
    }

    public void Interact(PlayerInteraction player)
    {
        Show();
    }

    void LobbyInterface_OnMenuToggle()
    {
        if (!IsComputerOpen && !LobbyInterface.Instance.IsInterfaceOpen)
            Show();
        else if (IsComputerOpen)
            Hide();
    }

    void Show()
    {
        _computerUI.Show();
        LobbyInterface.Instance.ToggleInterface(true);
    }

    void Close()
    {
        if (!IsComputerOpen)
            return;

        if (HasWindowOpen)
            CloseTopWindow();
        else
            Hide();
    }

    void Hide()
    {
        _computerUI.Hide();
        LobbyInterface.Instance.ToggleInterface(false);
    }

    void CloseTopWindow()
    {
        ComputerWindowUI window = _openedWindows[_openedWindows.Count - 1];
        window.Hide();
        CloseWindow(window);
    }

    /// <summary>
    /// Close a given computer window
    /// </summary>
    /// <param name="window"></param>
    public void CloseWindow(ComputerWindowUI window)
    {
        _openedWindows.Remove(window);
    }

    /// <summary>
    /// Add a given window to a list
    /// </summary>
    /// <param name="window"></param>
    public void AddWindow(ComputerWindowUI window)
    {
        if (!_openedWindows.Contains(window))
        {
            _openedWindows.Add(window);
        }
    }
}
