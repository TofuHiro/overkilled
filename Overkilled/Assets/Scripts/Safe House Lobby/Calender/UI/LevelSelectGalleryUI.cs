using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectGalleryUI : MonoBehaviour
{
    [Tooltip("Navigation button to go to the previous page")]
    [SerializeField] Button _prevPageButton;
    [Tooltip("Navigation button to go to the next page")]
    [SerializeField] Button _nextPageButton;
    [Tooltip("The parent for the set of month objects to cycle through")]
    [SerializeField] Transform _monthsParent;

    GameObject[] _monthPages;

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

        List<GameObject> pages = new List<GameObject>();
        foreach (Transform page in _monthsParent)
            pages.Add(page.gameObject);

        _monthPages = pages.ToArray();
    }

    void Start()
    {
        foreach (GameObject page in _monthPages)
            page.SetActive(false);

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
