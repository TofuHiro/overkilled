using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public static Computer Instance { get; private set; }

    public bool HasWindowOpen { get { return _openedWindows.Count > 0; } }

    [Tooltip("The computer UI to display")]
    [SerializeField] ComputerUI _computerUI;
    [Tooltip("Cover to prevent player from opening windows")]
    [SerializeField] GameObject _coverObject;

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
    }

    void OnDestroy()
    {
        LobbyInterface.Instance.OnUICancel -= Close;
    }

    public void Interact(PlayerInteraction player)
    {
        _computerUI.Show();

        LobbyInterface.Instance.ToggleInterface(true);
    }

    void Close()
    {
        if (HasWindowOpen)
        {
            CloseTopWindow();
        }
        else
        {
            _computerUI.Hide();

            LobbyInterface.Instance.ToggleInterface(false);
        }
    }

    void CloseTopWindow()
    {
        ComputerWindowUI window = _openedWindows[_openedWindows.Count - 1];
        window.Hide();
        CloseWindow(window);
    }

    public void CloseWindow(ComputerWindowUI window)
    {
        _openedWindows.Remove(window);

        _coverObject.SetActive(_openedWindows.Count > 0);
    }

    public void AddWindow(ComputerWindowUI window)
    {
        if (!_openedWindows.Contains(window))
        {
            _openedWindows.Add(window);
            _coverObject.SetActive(true);
        }
    }
}
