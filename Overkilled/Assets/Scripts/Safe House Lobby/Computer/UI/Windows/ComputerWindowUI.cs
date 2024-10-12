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

    protected virtual void Awake()
    {
        _closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        Computer.Instance.AddWindow(this);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        Computer.Instance.CloseWindow(this);
    }
}
