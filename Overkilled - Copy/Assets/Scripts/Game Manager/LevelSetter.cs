using UnityEngine;

/// <summary>
/// Singleton class used to preset a scene with a level preset. Set a LevelPreset to be loaded by the GameManager
/// </summary>
public class LevelSetter : MonoBehaviour
{
    [Tooltip("The level preset for this level scene")]
    [SerializeField] LevelPreset _preset;

    public static LevelSetter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LevelPreset found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public LevelPreset GetPreset()
    {
        return _preset;
    }
}
