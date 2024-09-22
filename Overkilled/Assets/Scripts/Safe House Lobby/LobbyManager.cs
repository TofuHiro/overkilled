using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [Tooltip("The local player reference the local player starts with")]
    [SerializeField] PlayerController _localPlayerInstance;
    [Tooltip("The position to spawn the local player when the scene starts")]
    [SerializeField] Vector3 _localPlayerSpawnPosition;

    public static LobbyManager Instance { get; private set; }

    public event Action OnSwitchToMultiplayer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of LobbyManager found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
    }

    void NetworkManager_OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (data.ClientId == NetworkManager.LocalClientId)
        {
            if (data.EventType == ConnectionEvent.ClientConnected)
            {
                _localPlayerInstance.gameObject.SetActive(false);
                OnSwitchToMultiplayer?.Invoke();
            }
        }
    }

    public void StartGame()
    {
        if (!PlayerReadyManager.Instance.AllPlayersReady)
            return;

        GameLobby.Instance.DeleteLobby();
        Loader.LoadSceneNetwork(Loader.Scene.GameScene);
    }

    public void ReloadLobby()
    {
        Destroy(NetworkManager.Singleton.gameObject);
        Destroy(MultiplayerManager.Instance.gameObject);
        SceneManager.LoadScene(Loader.Scene.SafeHouseScene.ToString());
    }
}
