using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
    public const string KEY_SELECTED_LEVEL = "SelectedLevel";
    const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static GameLobby Instance { get; private set; }

    public bool InLobby { get { return _joinedLobby != null; } }

    /// <summary>
    /// Invoked when lobby creation has started
    /// </summary>
    public event Action OnCreateLobbyStarted;
    /// <summary>
    /// Invoked when a lobby is successfully created
    /// </summary>
    public event Action OnCreateLobbySuccess;
    /// <summary>
    /// Invoked when lobby creation has failed
    /// </summary>
    public event Action OnCreateLobbyFailed;
    /// <summary>
    /// Invoked when an attempt to join a lobby has started
    /// </summary>
    public event Action OnJoinStarted;
    /// <summary>
    /// Invoked when quick join lobby attempt has failed
    /// </summary>
    public event Action OnQuickJoinFailed;
    /// <summary>
    /// Invoked when joining a lobby attempt by code/id has failed
    /// </summary>
    public event Action OnJoinFailed;
    /// <summary>
    /// Invoked when a lobby was successfully joined
    /// </summary>
    public event Action OnJoinSuccess;
    /// <summary>
    /// Invoked when a new list of lobbies was queried
    /// </summary>
    public event Action<List<Lobby>> OnLobbyListChanged;

    Lobby _joinedLobby;
    float _heartBeatTimer, _maxHeartBeatTime = 15f;
    float _listLobbiesTimer, _maxListLobbiesTime = 2f;
    float _changeLobbyLevelTimer, _maxUpdateLobbyTime = 1.1f;

    bool _hostLobbyIsPrivate;

    bool _autoChangeLobbyLevel;
    Level _hostSelectedLobbyLevel;

    string _lobbyNameSearch;
    Level _searchSelectedLobbyLevel;

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

    void Start()
    {
        MultiplayerManager.Instance.OnLocalDisconnect += MultiplayerManager_OnLocalDisconnect;
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;
    }

    void MultiplayerManager_OnLocalDisconnect()
    {
        _joinedLobby = null;
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        if (_joinedLobby == null)
            return;

        _hostSelectedLobbyLevel = level;
        _autoChangeLobbyLevel = true;
    }

    void Update()
    {
        HandleLobbyHeartBeat();
        HandlePeriodicListLobbies();
        HandleChangeLobbyLevel();
    }

    async void HandleLobbyHeartBeat()
    {
        if (!IsLobbyHost())
            return;

        _heartBeatTimer += Time.deltaTime;
        if (_heartBeatTimer >= _maxHeartBeatTime)
        {
            _heartBeatTimer = 0;

            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    void HandlePeriodicListLobbies()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
            return;
        if (_joinedLobby != null)
            return;
        if (SceneManager.GetActiveScene().name != Loader.Scene.SafeHouseScene.ToString())
            return;

        _listLobbiesTimer += Time.deltaTime;
        if (_listLobbiesTimer >= _maxListLobbiesTime)
        {
            _listLobbiesTimer = 0;
            ListLobbies();
        }
    }

    void HandleChangeLobbyLevel()
    {
        if (_joinedLobby == null)
            return;

        _changeLobbyLevelTimer += Time.deltaTime;
        if (_autoChangeLobbyLevel && _changeLobbyLevelTimer >= _maxUpdateLobbyTime)
        {
            UpdateLobbyLevel(_hostSelectedLobbyLevel);
            _changeLobbyLevelTimer = 0f;
            _autoChangeLobbyLevel = false;
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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_joinedLobby.MaxPlayers - 1);
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

    /// <summary>
    /// Create a new lobby with a given name and privacy modifier
    /// </summary>
    /// <param name="lobbyName">The name of the lobby</param>
    /// <param name="isPrivate">If the new lobby is private or public. Private lobbies require a code to join</param>
    public async void CreateLobby(string lobbyName, bool isPrivate, int maxPlayers)
    {
        OnCreateLobbyStarted?.Invoke();

        try
        {
            _hostLobbyIsPrivate = isPrivate;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_SELECTED_LEVEL, new DataObject(DataObject.VisibilityOptions.Public, _hostSelectedLobbyLevel.ToString(), DataObject.IndexOptions.S1) }
                },
            };

            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                },
            });

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            MultiplayerManager.Instance.StartHost();
            OnCreateLobbySuccess?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnCreateLobbyFailed?.Invoke();
        }
    }

    /// <summary>
    /// Quick join a random lobby
    /// </summary>
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            MultiplayerManager.Instance.StartClient();
            OnJoinSuccess?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnQuickJoinFailed?.Invoke();
        }
    }

    /// <summary>
    /// Attempt to join a lobby with a given code
    /// </summary>
    /// <param name="lobbyCode">The lobby code of the target lobby</param>
    public async void JoinLobbyWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            MultiplayerManager.Instance.StartClient();
            OnJoinSuccess?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke();
        }
    }

    /// <summary>
    /// Attempt to join a lobby with a given ID
    /// </summary>
    /// <param name="lobbyCode">The lobby ID of the target lobby</param>
    public async void JoinLobbyWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke();

        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            MultiplayerManager.Instance.StartClient();
            OnJoinSuccess?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke();
        }
    }

    /// <summary>
    /// Get the current lobby this player is in
    /// </summary>
    /// <returns></returns>
    public Lobby GetLobby()
    {
        return _joinedLobby;
    }

    /// <summary>
    /// Delete the current lobby the player is hosting
    /// </summary>
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

    /// <summary>
    /// Leave the current lobby this player is in
    /// </summary>
    public async Task LeaveLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                if (IsLobbyHost())
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                }

                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    /// <summary>
    /// Kick a target player given a player ID
    /// </summary>
    /// <param name="playerId">The ID of the player to kick from the lobby</param>
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

    /// <summary>
    /// Query a list of lobbies
    /// </summary>
    public async void ListLobbies()
    {
        try
        {
            bool nameEmpty = string.IsNullOrEmpty(_lobbyNameSearch);
            bool anyLevel = _searchSelectedLobbyLevel == Level.None;

            List<QueryFilter> filters = new List<QueryFilter>();
            if (!nameEmpty)
                filters.Add(new QueryFilter(QueryFilter.FieldOptions.Name, _lobbyNameSearch, QueryFilter.OpOptions.CONTAINS));
            if (!anyLevel)
                filters.Add(new QueryFilter(QueryFilter.FieldOptions.S1, _searchSelectedLobbyLevel.ToString(), QueryFilter.OpOptions.EQ));

            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 100,
                Filters = filters,
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

    public async void LockLobby()
    {
        if (_joinedLobby == null)
            return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                IsPrivate = true,
                IsLocked = true,
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async void UnlockLobby()
    {
        if (_joinedLobby == null)
            return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                IsPrivate = _hostLobbyIsPrivate,
                IsLocked = false,
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    async void UpdateLobbyLevel(Level level)
    {
        if (!IsLobbyHost())
            return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_SELECTED_LEVEL, new DataObject(DataObject.VisibilityOptions.Public, level.ToString(), DataObject.IndexOptions.S1) }
                },
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public void SetSearchSettings(string lobbyName, Level selectedLobbyLevel)
    {
        _lobbyNameSearch = lobbyName;
        _searchSelectedLobbyLevel = selectedLobbyLevel;
    }

    async void OnApplicationQuit()
    {
        await LeaveLobby();
    }
}
