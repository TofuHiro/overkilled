using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    [SerializeField] LevelPreset _preset;

    void Start()
    {
        GameManager.Instance.InitializeLevel(_preset);
    }
}
