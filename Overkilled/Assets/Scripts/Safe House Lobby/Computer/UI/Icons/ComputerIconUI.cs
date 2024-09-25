using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerIconUI : MonoBehaviour
{
    [Tooltip("The computer window this icon opens up")]
    [SerializeField] ComputerWindowUI _targetWindowUI;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            _targetWindowUI.Show();
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
