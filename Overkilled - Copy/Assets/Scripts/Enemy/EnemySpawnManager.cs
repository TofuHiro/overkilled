using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawnManager : NetworkBehaviour
{
    [System.Serializable]
    class EnemySpawn
    {
        [Tooltip("The set of enemy prefabs to spawn for this set. This can be a mix of the same enemy " +
            "with different variations or different enemies where Spawn Sequentially can be used to spawn " +
            "the set of enemies in that order.")]
        public GameObject[] EnemyPrefabs;

        [Tooltip("If true, the enemy prefabs will be cycled through sequentially and spawned in that order, " +
            "otherwise, enemies will be randomly picked from the EnemyPrefabs array.")]
        public bool SpawnSequentially;

        [Tooltip("The delay for the first enemy spawn")]
        public float StartSpawningDelay;

        [Tooltip("The frequency to spawn enemy after the first initial spawn")]
        public float SpawnFrequency;

        [Header("Spawn Points")]
        [Tooltip("The set of spawn points to use to spawn this set of enemies. The spawn points are chosen randomly")]
        public Transform[] EnemySpawnPoints;

        [Tooltip("If true, the spawn points set in the Enemy Spawn Points array will be highlighted with gizmo cubes")]
        public bool ShowSpawnPointGizmos;
        [Tooltip("For edit mode. Highlights spawn points with a given color")]
        public Color GizmoColor;

        public float NextTimeToSpawn { get; set; }

        int _spawnIndex = 0;

        /// <summary>
        /// Get the next enemy prefab to spawn
        /// </summary>
        /// <returns></returns>
        public GameObject GetNextEnemyPrefab() 
        {
            if (SpawnSequentially)
            {
                GameObject prefab = EnemyPrefabs[_spawnIndex++];
                _spawnIndex++;

                if (_spawnIndex > EnemyPrefabs.Length - 1)
                    _spawnIndex = 0;

                return prefab;
            }
            else
            {
                return EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
            }
        }

        /// <summary>
        /// Get a random spawn point transform
        /// </summary>
        /// <returns></returns>
        public Transform GetRandomSpawnPoint() { return EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Length)]; }
    }

    [Tooltip("Define enemy spawn presets")]
    [SerializeField] EnemySpawn[] _enemySpawns;
    [Tooltip("Base number of maximum number of enemies possible in the level")]
    [SerializeField] int _maxEnemyCountBase;
    [Tooltip("where constant = k, max enemies = base + (base * k * numOfPlayers). E.g base = 10, k = 0.5, numOfPlayers = 3, max = 10 + (10 * 0.5 * 3) = 10 + 15 = 25")]
    [SerializeField] float _enemyCountConstant = .2f;

    public static EnemySpawnManager Instance { get; private set; }

    bool _spawnerActive = false;
    float _timer = 0f;
    int _currentEnemyCount;
    int _maxEnemyCount;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of EnemySpawnManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        foreach (var enemySpawn in _enemySpawns)
            enemySpawn.NextTimeToSpawn = enemySpawn.StartSpawningDelay;

        int numOfPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        _maxEnemyCount = (int)(_maxEnemyCountBase + (_maxEnemyCountBase * _enemyCountConstant * numOfPlayers));

        EnemyHealth.OnDeath += EnemyHealth_OnDeath;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    // As this component is placed in level scene and not base scene, game manager will not be present until base scene is loaded (loaded after level scene)
    void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
        }
    }

    public override void OnNetworkDespawn()
    {
        EnemyHealth.OnDeath -= EnemyHealth_OnDeath;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    void EnemyHealth_OnDeath()
    {
        _currentEnemyCount--;
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.GameStarted)
            StartSpawning();
        else if (GameManager.Instance.GameEnded)
            StopSpawning();
    }

    void Update()
    {
        if (!IsServer)
            return;

        if (_currentEnemyCount >= _maxEnemyCount)
            return;

        if (_spawnerActive)
            _timer += Time.deltaTime;

        CheckToSpawn();
    }
    
    void StartSpawning()
    {
        _spawnerActive = true;
    }

    void StopSpawning()
    {
        _spawnerActive = false;
    }

    void CheckToSpawn()
    {
        foreach (EnemySpawn enemySpawn in _enemySpawns)
        {
            if (_timer >= enemySpawn.NextTimeToSpawn)
            {
                SpawnEnemy(enemySpawn);
                enemySpawn.NextTimeToSpawn = _timer + enemySpawn.SpawnFrequency;

                _currentEnemyCount++;
            }
        }
    }

    void SpawnEnemy(EnemySpawn enemySpawn)
    {
        Vector3 spawnPoint = enemySpawn.GetRandomSpawnPoint().position;
        GameObject enemyPrefab = enemySpawn.GetNextEnemyPrefab();
        ObjectSpawner.Instance.SpawnObject(enemyPrefab, spawnPoint, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        if (_enemySpawns.Length == 0)
            return;

        foreach (EnemySpawn enemySpawn in _enemySpawns)
        {
            if (enemySpawn.ShowSpawnPointGizmos)
            {
                foreach (Transform spawnPoint in enemySpawn.EnemySpawnPoints)
                {
                    Gizmos.color = enemySpawn.GizmoColor;
                    Gizmos.DrawCube(spawnPoint.position, new Vector3(.2f, 2f, .2f));
                }
            }
        }
    }
}
