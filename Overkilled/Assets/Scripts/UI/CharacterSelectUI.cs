using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] Button _readyButton;

    void Awake()
    {
        _readyButton.onClick.AddListener(() => 
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });    
    }
}
