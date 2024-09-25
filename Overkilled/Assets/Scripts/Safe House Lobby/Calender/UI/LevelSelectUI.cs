using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] Button _prevPageButton;
    [SerializeField] Button _nextPageButton;
    [SerializeField] GameObject[] _monthPages;

    int _pageIndex = 0;

    void Awake()
    {
        _prevPageButton.onClick.AddListener(() =>
        {
            SwitchPage(_pageIndex - 1);
        });

        _nextPageButton.onClick.AddListener(() =>
        {
            SwitchPage(_pageIndex + 1);
        });
    }

    void Start()
    {
        _monthPages[0].SetActive(true);

        Hide();
    }

    void SwitchPage(int index)
    {
        _monthPages[_pageIndex].SetActive(false);

        if (index > 11)
            _pageIndex = 0;
        else if (index < 0)
            _pageIndex = 11;
        else
            _pageIndex = index;

        _monthPages[_pageIndex].SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
