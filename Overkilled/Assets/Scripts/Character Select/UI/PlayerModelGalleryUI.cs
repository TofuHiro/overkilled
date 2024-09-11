using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModelGalleryUI : MonoBehaviour
{
    [SerializeField] Button _leftButton;
    [SerializeField] Button _rightButton;

    void Awake()
    {
        _leftButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.CyclePlayerModel(false);
        });

        _rightButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.CyclePlayerModel(true);
        });
    }
}
