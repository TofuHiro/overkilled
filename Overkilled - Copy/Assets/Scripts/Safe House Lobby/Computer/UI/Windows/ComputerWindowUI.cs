using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerWindowUI : MonoBehaviour
{
    [Header("Computer Window")]
    [Tooltip("The close button on the window")]
    [SerializeField] Button _closeButton;

    public bool IsOpen { get; private set; }

    protected virtual void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    public virtual void Show()
    {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        IsOpen = false;
        gameObject.SetActive(false);
    }

    protected void Close()
    {
        Computer.Instance.CloseWindow(this);
    }

    public virtual void Back()
    {
        //default - override this
        Close();
    }
}
