using SurvivalGame;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    const int MAX_PLAYER_COUNT = 4;

    [SerializeField] NetworkPrefabsList _networkPrefabsList;

    public static MultiplayerManager Instance { get; private set; }

    public static event Action OnDisconnect;
    public static event Action OnTryingToJoinGame;
    public static event Action OnFailedToJoinGame;

    NetworkObject _previousSpawnedObject;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnDisconnectEvent;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnConnectionEvent -= OnDisconnectEvent;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += SetConnectionApproval;
        NetworkManager.Singleton.StartHost();
    }

    void SetConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }
        
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT)
        {
            response.Approved = false;
            response.Reason = "Lobby is full";
            return;
        }

        response.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke();

        NetworkManager.Singleton.OnConnectionEvent += FailToJoin;
        NetworkManager.Singleton.StartClient();
    }

    void FailToJoin(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
            OnFailedToJoinGame?.Invoke();
    }

    void OnDisconnectEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.EventType == ConnectionEvent.ClientDisconnected && data.ClientId == NetworkManager.Singleton.LocalClientId)
            OnDisconnect?.Invoke();
    }

    int GetIndexFromObject(GameObject obj)
    {
        for (int i = 0; i < _networkPrefabsList.PrefabList.Count; i++)
            if (_networkPrefabsList.PrefabList[i].Prefab.name == obj.name)
                return i;

        return -1;
    }

    GameObject GetObjectFromIndex(int index)
    {
        return _networkPrefabsList.PrefabList[index].Prefab;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The network object of the spawned item</returns>
    public NetworkObject SpawnItem(GameObject item)
    {
        SpawnItemServerRpc(GetIndexFromObject(item));
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position">The position to spawn at</param>
    /// <param name="rotation">The rotation to spawn with</param>
    /// <returns></returns>
    public NetworkObject SpawnItem(GameObject item, Vector3 position, Quaternion rotation)
    {
        SpawnItem(item);
        _previousSpawnedObject.transform.position = position;
        _previousSpawnedObject.transform.rotation = rotation;
        return _previousSpawnedObject;
    }

    /// <summary>
    /// Instantiates a given item in multiplayer
    /// </summary>
    /// <param name="item"></param>
    /// <param name="hand">The hand to set the items parents to</param>
    /// <returns></returns>
    public NetworkObject SpawnItem(GameObject item, PlayerHand hand)
    {
        SpawnItemWithParentServerRpc(GetIndexFromObject(item), hand.GetNetworkObject());
        return _previousSpawnedObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemServerRpc(int objectIndex)
    {
        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        _previousSpawnedObject = networkObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemWithParentServerRpc(int objectIndex, NetworkObjectReference handNetworkObjectReference)
    {
        handNetworkObjectReference.TryGet(out NetworkObject handNetworkObject);

        GameObject material = Instantiate(GetObjectFromIndex(objectIndex));
        NetworkObject networkObject = material.GetComponent<NetworkObject>();
        networkObject.GetComponent<NetworkObject>().Spawn(true);

        PlayerHand hand = handNetworkObject.GetComponent<PlayerHand>();
        hand.SetItem(networkObject.GetComponent<Item>());

        _previousSpawnedObject = networkObject;
    }

    public void DestroyItem(GameObject item)
    {
        DestroyItemServerRpc(item.GetComponent<NetworkObject>(), 0);
    }

    public void DestroyItem(GameObject item, float delay)
    {
        DestroyItemServerRpc(item.GetComponent<NetworkObject>(), delay);
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyItemServerRpc(NetworkObjectReference itemNetworkObjectReference, float delay)
    {
        itemNetworkObjectReference.TryGet(out NetworkObject itemNetworkObject);
        Destroy(itemNetworkObject.gameObject, delay);
    }
}
