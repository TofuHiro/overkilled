using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
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

            await UnityServices.InitializeAsync();
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

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke();

        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Name") }
                    }
                }
            };

            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYER_COUNT, options);

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
