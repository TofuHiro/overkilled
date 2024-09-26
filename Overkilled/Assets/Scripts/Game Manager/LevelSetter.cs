using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    [SerializeField] LevelPreset _preset;

    public static LevelSetter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LevelPreset found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    public LevelPreset GetPreset()
    {
        return _preset;
    }
}
