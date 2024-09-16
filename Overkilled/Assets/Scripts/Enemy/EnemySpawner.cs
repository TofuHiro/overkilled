using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("The enemy to spawn")]
    [SerializeField] GameObject _enemyPrefab;

    MultiplayerManager _multiplayerManager;

    void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }

    public void Spawn()
    {
        _multiplayerManager.SpawnObject(_enemyPrefab, transform.position, transform.rotation);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(.2f, .4f, .2f));    
    }
}
