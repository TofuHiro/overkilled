using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [Tooltip("Network prefab list used for spawning objects. It is used to find the index of an item by reference and spawned from this list")]
    [SerializeField] NetworkPrefabsList _networkPrefabsList;

    public static ObjectSpawner Instance { get; private set; }

    readonly Dictionary<string, int> _objectIndices = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of ObjectSpawner found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        for (int i = 0; i < _networkPrefabsList.PrefabList.Count; i++)
        {
            _objectIndices.Add(_networkPrefabsList.PrefabList[i].Prefab.name, i);
        }
    }

    int GetIndexFromObject(GameObject obj)
    {
        string objName = obj.name.Replace("(Clone)", "");
        int objIndex = _objectIndices.ContainsKey(objName) ? _objectIndices[objName] : -1;

        if (objIndex < 0)
            Debug.LogError($"{obj.name} is not found in object dictionary");

        return objIndex;
    }

    GameObject GetObjectFromIndex(int index)
    {
        return _networkPrefabsList.PrefabList[index].Prefab;
    }

    /// <summary>
    /// Instantiates a given object and spawns it in multiplayer
    /// </summary>
    public void SpawnObject(GameObject obj)
    {
        SpawnObjectServerRpc(GetIndexFromObject(obj), Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Instantiates a given object and spawns it in multiplayer
    /// </summary>
    public void SpawnObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        SpawnObjectServerRpc(GetIndexFromObject(obj), position, rotation);
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectServerRpc(int objectIndex, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetObjectFromIndex(objectIndex);
        NetworkObject networkObject;

        if (NetworkObjectPool.Instance.HasPool(obj))
        {
            networkObject = NetworkObjectPool.Instance.GetNetworkObject(obj, position, rotation);
        }
        else
        {
            networkObject = Instantiate(obj, position, rotation).GetComponent<NetworkObject>();
        }

        networkObject.Spawn(true);
    }

    /// <summary>
    /// Instantiates a given object and spawns it in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="hand">The hand to set the items parents to</param>
    public void SpawnObject(GameObject obj, PlayerHand hand)
    {
        if (hand.IsHoldingItem || !hand.GetNetworkObject())
            return;

        SpawnObjectWithParentServerRpc(GetIndexFromObject(obj), hand.GetNetworkObject());
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectWithParentServerRpc(int objectIndex, NetworkObjectReference handNetworkObjectReference)
    {
        handNetworkObjectReference.TryGet(out NetworkObject handNetworkObject);
        PlayerHand hand = handNetworkObject.GetComponent<PlayerHand>();

        if (hand.IsHoldingItem)
            return;

        GameObject obj = GetObjectFromIndex(objectIndex);
        NetworkObject networkObject;

        if (NetworkObjectPool.Instance.HasPool(obj))
        {
            networkObject = NetworkObjectPool.Instance.GetNetworkObject(obj, hand.transform.position, hand.transform.rotation);
        }
        else
        {
            networkObject = Instantiate(obj, hand.transform.position, hand.transform.rotation).GetComponent<NetworkObject>();
        }

        networkObject.Spawn(true);
        hand.SetItem(networkObject.GetComponent<Item>());
    }

    /// <summary>
    /// Destroy a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    public void DestroyObject(GameObject obj)
    {
        NetworkObject networkObject = obj.GetComponent<NetworkObject>();
        
        if (networkObject)
        {
            DestroyObjectServerRpc(networkObject, 0);
        }
        else
        {
            Debug.LogError("Network Object not found on object: " + obj.name);
        }
    }

    /// <summary>
    /// Destroy a given object in multiplayer
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="delay">The time delay to destroy this object</param>
    public void DestroyObject(GameObject obj, float delay)
    {
        NetworkObject networkObject = obj.GetComponent<NetworkObject>();

        if (networkObject)
        {
            DestroyObjectServerRpc(networkObject, delay);
        }
        else
        {
            Debug.LogError("Network Object not found on object: " + obj.name);
        }
    }

    [Rpc(SendTo.Server)]
    void DestroyObjectServerRpc(NetworkObjectReference networkObjectReference, float delay)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);

        // If already destroyed
        if (networkObject == null)
            return;

        StartCoroutine(DestroyObjectDelay(networkObject, delay));
    }

    IEnumerator DestroyObjectDelay(NetworkObject networkObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        networkObject.Despawn();
    }
}
