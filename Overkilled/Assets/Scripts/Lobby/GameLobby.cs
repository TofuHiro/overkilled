using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
    const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static GameLobby Instance { get; private set; }

    public event Action OnCreateLobbyStarted;
    public event Action OnCreateLobbyFailed;
    public event Action OnJoinStarted;
    public event Action OnQuickJoinFailed;
    public event Action OnJoinFailed;
    public event Action<List<Lobby>> OnLobbyListChanged;

    Lobby _joinedLobby;
    float _heartBeatTimer, _maxHeartBeatTime = 20f;
    float _listLobbiesTimer, _maxListLobbiesTimer = 3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());////

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void Update()
    {
        HandleLobbyHeartBeat();
        HandlePeriodicListLobbies();
    }

    async void HandleLobbyHeartBeat()
    {
        if (!IsLobbyHost())
            return;

        _heartBeatTimer += Time.deltaTime;
        if (_heartBeatTimer >= _maxHeartBeatTime)
        {
            _heartBeatTimer = 0;
            await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
        }
    }

    void HandlePeriodicListLobbies()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
            return;
        if (_joinedLobby != null)
            return;
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString())
            return;

        _listLobbiesTimer += Time.deltaTime;
        if (_listLobbiesTimer >= _maxListLobbiesTimer)
        {
            _listLobbiesTimer = 0;
            ListLobbies();
        }
    }

    bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MAX_PLAYER_COUNT - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
            return default;
        }
    }

    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
            return default;
        }
    }

    async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke();

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                
            };

            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYER_COUNT, options);

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                },
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerManager.Instance.StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnCreateLobbyFailed?.Invoke();
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnQuickJoinFailed?.Invoke();
        }
    }

    public async void JoinLobbyWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke();
        }
    }

    public async void JoinLobbyWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke();
        }
    }

    public Lobby GetLobby()
    {
        return _joinedLobby;
    }

    public async void DeleteLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 20,
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
            },
                Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.AvailableSlots),
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);
            OnLobbyListChanged?.Invoke(queryResponse.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }
}
