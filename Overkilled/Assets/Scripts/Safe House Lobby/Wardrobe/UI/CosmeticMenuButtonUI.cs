using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticMenuButtonUI : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _selectedButtonOverlay;

    static GameObject s_selectedButtonOverlay;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Show);  
    }

    void Start()
    {
        _menu.SetActive(false);
        _selectedButtonOverlay.SetActive(false);
    }

    public void Show()
    {
        CharacterCustomizationUI.Instance.OpenMenu(_menu);

        if (s_selectedButtonOverlay != null)
            s_selectedButtonOverlay.SetActive(false);

        s_selectedButtonOverlay = _selectedButtonOverlay;
        _selectedButtonOverlay.SetActive(true);
    }
}
