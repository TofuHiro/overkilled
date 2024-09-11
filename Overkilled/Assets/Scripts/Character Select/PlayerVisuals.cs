using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] GameObject[] _playerModels;

    int _currentIndex;

    public void SetPlayerModel(int modelIndex)
    {
        _playerModels[_currentIndex].SetActive(false);
        _playerModels[modelIndex].SetActive(true);

        _currentIndex = modelIndex;
    }
}
