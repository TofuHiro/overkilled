using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

[Serializable]
struct PoolConfigObject
{
    public GameObject Prefab;
    public int PrewarmCount;
}

public class NetworkObjectPool : NetworkBehaviour
{
    [SerializeField] List<PoolConfigObject> _pooledPrefabsList;

    public static NetworkObjectPool Instance { get; private set; }

    HashSet<GameObject> _prefabs = new HashSet<GameObject>();
    Dictionary<GameObject, IObjectPool<NetworkObject>> _pooledObjects = new Dictionary<GameObject, IObjectPool<NetworkObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Bank found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool HasPool(GameObject prefab)
    {
        return _prefabs.Contains(prefab);
    }

    public override void OnNetworkSpawn()
    {
        foreach (var configObject in _pooledPrefabsList)
        {
            RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
        }
    }

    public override void OnNetworkDespawn()
    {
        foreach (var prefab in _prefabs)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
            _pooledObjects[prefab].Clear();
        }
        _prefabs.Clear();
        _pooledObjects.Clear();
    }

    public void OnValidate()
    {
        for (var i = 0; i < _pooledPrefabsList.Count; i++)
        {
            var prefab = _pooledPrefabsList[i].Prefab;
            if (prefab != null)
            {
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
            }
        }
    }

    /// <summary>
    /// Get a given object from the pool.
    /// </summary>
    /// <returns>The network object of the new object. Can be used to spawn</returns>
    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        NetworkObject networkObject = _pooledObjects[prefab].Get();

        networkObject.transform.position = position;
        networkObject.transform.rotation = rotation;

        return networkObject;
    }

    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        _pooledObjects[prefab].Release(networkObject);
    }

    void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
    {
        NetworkObject CreateFunc()
        {
            return Instantiate(prefab).GetComponent<NetworkObject>();
        }

        void ActionOnGet(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(true);
        }

        void ActionOnRelease(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(false);
        }

        void ActionOnDestroy(NetworkObject networkObject)
        {
            Destroy(networkObject.gameObject);
        }

        _prefabs.Add(prefab);

        // Create the pool
        _pooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

        // Populate the pool
        List<NetworkObject> prewarmNetworkObjects = new List<NetworkObject>();
        for (int i = 0; i < prewarmCount; i++)
        {
            prewarmNetworkObjects.Add(_pooledObjects[prefab].Get());
        }
        foreach (NetworkObject networkObject in prewarmNetworkObjects)
        {
            _pooledObjects[prefab].Release(networkObject);
        }
        
        NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
    }
}

class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    GameObject _prefab;
    NetworkObjectPool _pool;

    public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
    {
        _prefab = prefab;
        _pool = pool;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return _pool.GetNetworkObject(_prefab, position, rotation);
    }

    public void Destroy(NetworkObject networkObject)
    {
        _pool.ReturnNetworkObject(networkObject, _prefab);
    }
}
