using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] PlayerModel[] _playerModels;

    int _currentIndex;

    public void SetPlayerModel(int modelIndex)
    {
        _playerModels[_currentIndex].Hide(); ;
        _playerModels[modelIndex].Show();

        _currentIndex = modelIndex;
    }
}
