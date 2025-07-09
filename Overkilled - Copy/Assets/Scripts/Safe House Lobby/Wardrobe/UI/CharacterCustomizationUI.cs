using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] CosmeticMenuButtonUI _defaultMenuButton;
    
    public static CharacterCustomizationUI Instance { get; private set; }

    static GameObject s_currentMenu;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of CharacterCustomizationUI found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        _defaultMenuButton.Show();
       
        Hide();
    }

    public void OpenMenu(GameObject newMenu)
    {
        if (s_currentMenu != null) 
            s_currentMenu.SetActive(false);

        s_currentMenu = newMenu;
        newMenu.SetActive(true);
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
