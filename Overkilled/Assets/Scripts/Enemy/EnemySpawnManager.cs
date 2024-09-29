using SurvivalGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour, IStartInvoke
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

        [Tooltip("The set of spawn points to use to spawn this set of enemies. The spawn points are chosen randomly")]
        public Transform[] EnemySpawnPoints;

        [Tooltip("If true, the spawn points set in the Enemy Spawn Points array will be highlighted with gizmo cubes")]
        public bool ShowSpawnPointGizmos;

        [Tooltip("The frequency to spawn an enemy in this set")]
        public float SpawnFrequency;

        float _nextTimeToSpawn;
        int _spawnIndex = 0;

        /// <summary>
        /// Get the next time to spawn an enemy for this set
        /// </summary>
        /// <returns></returns>
        public float GetNextTimeToSpawn() { return _nextTimeToSpawn; }

        /// <summary>
        /// Set the next time to spawn an enemy for this set
        /// </summary>
        /// <param name="time"></param>
        public void SetNextTimeToSpawn(float time) { _nextTimeToSpawn = time; }

        /// <summary>
        /// Get the next enemy prefab to spawn
        /// </summary>
        /// <returns></returns>
        public GameObject GetNextEnemyPrefab() 
        {
            if (SpawnSequentially)
            {
                GameObject prefab = EnemyPrefabs[_spawnIndex];
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

    [SerializeField] EnemySpawn[] _enemySpawns;

    public static EnemySpawnManager Instance { get; private set; }

    bool _spawnerActive = false;
    float _timer = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of EnemySpawnManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;

        foreach (var enemySpawn in _enemySpawns)
            enemySpawn.SetNextTimeToSpawn(enemySpawn.SpawnFrequency);
    }

    public void InvokeStart()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    void Update()
    {
        if (_spawnerActive)
            _timer += Time.deltaTime;

        CheckToSpawn();
    }

    void GameManager_OnGameStateChange()
    {
        if (GameManager.Instance.GameStarted)
            StartSpawning();
        else if (GameManager.Instance.GameEnded)
            StopSpawning();
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
            if (_timer >= enemySpawn.GetNextTimeToSpawn())
            {
                SpawnEnemy(enemySpawn);
                enemySpawn.SetNextTimeToSpawn(_timer + enemySpawn.SpawnFrequency);
            }
        }
    }

    void SpawnEnemy(EnemySpawn enemySpawn)
    {
        Vector3 spawnPoint = enemySpawn.GetRandomSpawnPoint().position;
        GameObject enemyPrefab = enemySpawn.GetNextEnemyPrefab();
        MultiplayerManager.Instance.SpawnObject(enemyPrefab, spawnPoint, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        if (_enemySpawns.Length == 0)
            return;

        Gizmos.color = Color.red;

        foreach (EnemySpawn enemySpawn in _enemySpawns)
            if (enemySpawn.ShowSpawnPointGizmos)
                foreach (Transform spawnPoint in enemySpawn.EnemySpawnPoints)
                    Gizmos.DrawCube(spawnPoint.position, new Vector3(.2f, .4f, .2f));
    }
}
