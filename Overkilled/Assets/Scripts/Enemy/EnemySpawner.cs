using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;

    public void Spawn()
    {
        SpawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnServerRpc()
    {
        EnemyController enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation).GetComponentInChildren<EnemyController>();
        NetworkObject networkObject = enemy.GetNetworkObject();

        networkObject.Spawn(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(.2f, .4f, .2f));    
    }
}
