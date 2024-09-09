using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;

    MultiplayerManager _multiplayerManager;

    void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }

    public void Spawn()
    {
        _multiplayerManager.SpawnItem(_enemyPrefab, transform.position, transform.rotation);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(.2f, .4f, .2f));    
    }
}
