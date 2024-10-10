using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMenuButtonUI : MonoBehaviour
{
    [SerializeField] QuickMenuUI _quickMenuUI;
    [SerializeField] ComputerWindowUI _targetWindow;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            _targetWindow.Show();
            _quickMenuUI.Hide();
        });
    }

    protected void Show()
    {
        gameObject.SetActive(true);
    }

    protected void Hide()
    {
        gameObject.SetActive(false);
    }
}
