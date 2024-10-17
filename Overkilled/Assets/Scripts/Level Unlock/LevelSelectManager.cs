using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelSelectManager : NetworkBehaviour, IDataPersistence
{
    public static LevelSelectManager Instance { get; private set; }
    /// <summary>
    /// The current level that is being played
    /// </summary>
    public Level CurrentLevel { get { return _currentLevel.Value; } }

    public delegate void LevelSelectAction(Level level);
    /// <summary>
    /// Invoked when the selected level is changed
    /// </summary>
    public event LevelSelectAction OnLevelSelectChange;
    /// <summary>
    /// Invoked when a level is unlocked. The level enum of the unlocked level is passed a parameter. 
    /// This is a static event as it is called before subscription, so to fix, subscribe to this in Awake method and ensure to unsubscribe
    /// </summary>
    public static event LevelSelectAction OnLevelUnlock;

    NetworkVariable<Level> _currentLevel = new NetworkVariable<Level>();

    SerializableDictionary<Level, LevelInfo> _levelInfos;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LevelSelectManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LobbyManager.OnLobbyLoad += LobbyManager_OnLobbyLoad;
    }

    public override void OnDestroy()
    {
        LobbyManager.OnLobbyLoad -= LobbyManager_OnLobbyLoad;
    }

    void LobbyManager_OnLobbyLoad()
    {
        UnlockLevels();
    }

    /// <summary>
    /// Set the selected level to a given level
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(Level level)
    {
        OnLevelSelectChange?.Invoke(level);

        if (IsServer)
        {
            _currentLevel.Value = level;
            SetLevelRpc(level);
        }
    }

    [Rpc(SendTo.NotMe)]
    void SetLevelRpc(Level level)
    {
        OnLevelSelectChange?.Invoke(level);
    }

    /// <summary>
    /// Mark the current level being played as complete, saving the current score and grade
    /// </summary>
    /// <param name="currentScore">The final score achieved</param>
    /// <param name="currentGrade">The final grade achieved</param>
    public void CompleteCurrentLevel(int currentScore, Grade currentGrade)
    {
        if (CurrentLevel == Level.None)
            return;

        if (_levelInfos.ContainsKey(CurrentLevel))
        {
            int highestScore = _levelInfos[CurrentLevel].HighestScore;
            Grade highestGrade = _levelInfos[CurrentLevel].HighestGrade;

            if (currentScore > highestScore)
                _levelInfos[CurrentLevel].HighestScore = currentScore;
            if (currentGrade > highestGrade)
                _levelInfos[CurrentLevel].HighestGrade = currentGrade;
        }
        else
        {
            //If level does not exists, it is never unlocked, so just save scores and leave unlocked
            _levelInfos.Add(CurrentLevel, new LevelInfo(CurrentLevel, false, currentScore, currentGrade));
        }
        
        if (_levelInfos[CurrentLevel].Unlocked)
            UnlockNextLevel();
    }

    void UnlockNextLevel()
    {
        Level nextLevel = CurrentLevel + 1;

        if (GetNextLockedLevel() == nextLevel)
        {
            if (!_levelInfos.ContainsKey(nextLevel))
                _levelInfos.Add(nextLevel, new LevelInfo(nextLevel, true, 0, Grade.NoStars));
            else
                _levelInfos[nextLevel].Unlocked = true;

            OnLevelUnlock?.Invoke(nextLevel);
        }
    }

    public void SaveGame(ref GameData data)
    {
        data.LevelInfos = _levelInfos;
    }

    public void LoadGame(GameData data)
    {
        _levelInfos = data.LevelInfos;
    }

    void UnlockLevels()
    {
        foreach (var level in _levelInfos)
        {
            if (level.Value.Unlocked)
            {
                OnLevelUnlock?.Invoke(level.Key);
            }
        }
    }

    /// <summary>
    /// Returns the Level enum of the next locked level
    /// </summary>
    /// <returns></returns>
    Level GetNextLockedLevel()
    {
        foreach (Level level in System.Enum.GetValues(typeof(Level)))
        {
            if (level != Level.None)
            {
                if (!_levelInfos.ContainsKey(level))
                {
                    return level;
                }
                else if (!_levelInfos[level].Unlocked)
                {
                    return level;
                }
            }
        }

        return Level.None;
    }
}
