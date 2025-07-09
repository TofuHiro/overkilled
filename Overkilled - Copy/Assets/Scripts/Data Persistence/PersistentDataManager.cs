using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    [Tooltip("The file name for the game data save file")]
    [SerializeField] string _fileName;
    [Tooltip("Whether to use encryption for the save file")]
    [SerializeField] bool _useFileEncryption;

    public static PersistentDataManager Instance { get; private set; }

    /// <summary>
    /// Invoked when the game is successfully saved
    /// </summary>
    public event Action OnSave;

    FileDataHandler _dataHandler;
    GameData _gameData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of PersistentDataManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
#if DEVELOPMENT_BUILD
        _dataHandler = new FileDataHandler(Application.persistentDataPath, "build.json", _useFileEncryption);
#else      
        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useFileEncryption);
#endif

        LoadGame();
    }

    void NewGame()
    {
        _gameData = new GameData();
    }

    /// <summary>
    /// Saves the game with all persistent objects
    /// </summary>
    public void SaveGame()
    {
        Debug.Log("Saving Game...");

        foreach (IDataPersistence dataPersistenceObject in GetDataPersistenceObjects())
        {
            dataPersistenceObject.SaveGame(ref _gameData);
        }

        _dataHandler.Save(_gameData);

        OnSave?.Invoke();
    }

    /// <summary>
    /// Loads the save file and loads all persistent objects
    /// </summary>
    public void LoadGame()
    {
        Debug.Log("Loading Game...");

        _gameData = _dataHandler.Load();

        if (_gameData == null)
        {
            Debug.LogWarning("No current GameData found. Creating new GameData");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObject in GetDataPersistenceObjects())
        {
            dataPersistenceObject.LoadGame(_gameData);
        }
    }

    List<IDataPersistence> GetDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
