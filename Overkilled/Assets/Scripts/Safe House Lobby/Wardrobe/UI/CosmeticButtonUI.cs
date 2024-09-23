using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticButtonUI : MonoBehaviour
{
    [SerializeField] CosmeticSO _cosmeticSO;
    [SerializeField] Image _icon;
    [SerializeField] TMP_Text _unlockText;

    void Awake()
    {
        /*_icon.sprite = _cosmeticSO.icon;
        _unlockText.text = _cosmeticSO.unlockText;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            
        });*/
    }
}
