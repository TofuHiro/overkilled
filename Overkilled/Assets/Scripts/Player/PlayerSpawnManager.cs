using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Tooltip("Transforms at positions where a player may spawn.")]
    [SerializeField] Transform[] _spawnPositions;
    [Tooltip("For debugging. Draws cubes at all spawn positions")]
    [SerializeField] bool _showSpawnPositions;

    public static PlayerSpawnManager Instance { get; private set; }

    public int SpawnIndex
    {
        get { return _spawnIndex; }
        set
        {
            _spawnIndex = value;

            if (_spawnIndex > _spawnPositions.Length - 1)
            {
                _spawnIndex = 0;
            }
        }
    }
    int _spawnIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of PlayerSpawnManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    public Vector3 GetNextSpawnPosition()
    {
        return _spawnPositions[SpawnIndex++].position;
    }

    void OnDrawGizmos()
    {
        if (!_showSpawnPositions)
            return;
        if (_spawnPositions == null || _spawnPositions.Length == 0)
            return;

        foreach (Transform spawnTransform in _spawnPositions)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(spawnTransform.position, Vector3.one * .5f);
        }
    }
}
