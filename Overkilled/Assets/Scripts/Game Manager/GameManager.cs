using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivalGame
{
    public class GameManager : NetworkBehaviour
    {
        public enum GameState
        {
            WaitingForPlayers,
            StartingGame,
            GameStarted,
            GameEnded,
        }

        [Tooltip("The player prefab to spawn as a player")]
        [SerializeField] GameObject _playerPrefab;
        [Tooltip("The count down timer when the game is starting")]
        [SerializeField] float _gameStartCountdownTime = 3f;

        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Returns true if the game is waiting for players to ready
        /// </summary>
        public bool IsWaiting { get { return _currentGameState.Value == GameState.WaitingForPlayers; } }

        /// <summary>
        /// Returns true if the game is starting or counting down to start
        /// </summary>
        public bool IsStarting { get { return _currentGameState.Value == GameState.StartingGame; } }

        /// <summary>
        /// Returns true if the game has started
        /// </summary>
        public bool GameStarted { get { return _currentGameState.Value == GameState.GameStarted; } }

        /// <summary>
        /// Returns true if the game has ended
        /// </summary>
        public bool GameEnded { get { return _currentGameState.Value == GameState.GameEnded; } }

        /// <summary>
        /// Returns true if the game is currently paused
        /// </summary>
        public bool IsPaused { get { return _isGamePaused.Value; } }

        public delegate void GameInitialize(LevelPreset preset);
        /// <summary>
        /// Invoked when the game settings are initialized
        /// </summary>
        public event GameInitialize OnGameInitialize;
        /// <summary>
        /// Invoked when the game's state is changed
        /// </summary>
        public event Action OnGameStateChange;
        /// <summary>
        /// Invoked when the local player is ready
        /// </summary>
        public event Action OnLocalPlayerReady;
        /// <summary>
        /// Invoked when the local player attempts to pause
        /// </summary>
        public event Action OnLocalGamePause;
        /// <summary>
        /// Invoked when the local player attempts to unpause
        /// </summary>
        public event Action OnLocalGameUnpause;
        /// <summary>
        /// Invoked when the game is set to pause
        /// </summary>
        public event Action OnMultiplayerGamePause;
        /// <summary>
        /// Invoked when the game is set to unpause
        /// </summary>
        public event Action OnMultiplayerGameUnpause;

        Dictionary<ulong, bool> _playerReadyDictionary;
        Dictionary<ulong, bool> _playerPausedDictionary;

        NetworkVariable<GameState> _currentGameState = new NetworkVariable<GameState>();
        NetworkVariable<Grade> _finalGrade = new NetworkVariable<Grade>();
        NetworkVariable<float> _countdownTimer = new NetworkVariable<float>();
        NetworkVariable<float> _gameTimer = new NetworkVariable<float>();
        NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
        NetworkVariable<bool> _gameFailed = new NetworkVariable<bool>(false);

        bool _isLocalPlayerReady = false, _isLocalPlayerPaused = false;
        bool _autoTestGamePausedState, _autoTestGameReadyStart;
        
        LevelPreset _currentLevelPreset;

        /// <summary>
        /// Returns the calculated grade for the game
        /// </summary>
        /// <returns></returns>
        public Grade GetGrade()
        {
            return _finalGrade.Value;
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Warning. Multiple instances of GameManager found. Destroying " + name);
                Destroy(Instance);
            }

            Instance = this;

            _playerReadyDictionary = new Dictionary<ulong, bool>();
            _playerPausedDictionary = new Dictionary<ulong, bool>();
        }

        public override void OnNetworkSpawn()
        {
            PlayerController.OnPlayerSpawn += PlayerController_OnPlayerSpawn;

            _currentGameState.OnValueChanged += OnStateChange;
            _isGamePaused.OnValueChanged += OnGamePausedChange;

            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent += TestPauseOnPlayerDisconnect;
                NetworkManager.Singleton.OnConnectionEvent += TestPlayersReadyOnPlayerDisconnect;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnPlayers;
            }
        }

        public override void OnNetworkDespawn()
        {
            PlayerController.OnPlayerSpawn -= PlayerController_OnPlayerSpawn;

            Time.timeScale = 1f;
            _currentGameState.OnValueChanged -= OnStateChange;
            _isGamePaused.OnValueChanged -= OnGamePausedChange;

            if (IsServer)
            {
                NetworkManager.Singleton.OnConnectionEvent -= TestPauseOnPlayerDisconnect;
                NetworkManager.Singleton.OnConnectionEvent -= TestPlayersReadyOnPlayerDisconnect;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnPlayers;
            }
        }

        void Start()
        {
            Init();
        }

        void Init()
        {
            _currentLevelPreset = LevelSetter.Instance.GetPreset();

            LevelCameraController.Instance.SetFocusMode(_currentLevelPreset.useCameraFocusMode);

            if (IsServer)
                _currentGameState.Value = GameState.WaitingForPlayers;
                _countdownTimer.Value = _gameStartCountdownTime;
                _gameTimer.Value = _currentLevelPreset.timeLimit;
            
            
            OnGameInitialize?.Invoke(_currentLevelPreset);
        }

        void PlayerController_OnPlayerSpawn(PlayerController player)
        {
            player.OnPlayerInteractInput += SetLocalPlayerReady;
            player.OnPauseInput += TogglePauseGame;
        }

        void SpawnPlayers(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                GameObject playerObject = Instantiate(_playerPrefab, PlayerSpawnManager.Instance.GetNextSpawnPosition(), Quaternion.identity);
                playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnPlayers;
        }

        void Update()
        {
            if (!IsServer)
                return;

            switch (_currentGameState.Value)
            {
                case GameState.WaitingForPlayers:
                    break;

                case GameState.StartingGame:
                    _countdownTimer.Value -= Time.deltaTime;
                    Debug.Log(_countdownTimer.Value);

                    if (_countdownTimer.Value <= 0)
                        _currentGameState.Value = GameState.GameStarted;
                    break;

                case GameState.GameStarted:
                    _gameTimer.Value -= Time.deltaTime;

                    if (_gameTimer.Value <= 0f)
                        _currentGameState.Value = GameState.GameEnded;
                    break;

                case GameState.GameEnded:
                    break;
            }
        }

        void LateUpdate()
        {
            if (_autoTestGameReadyStart)
            {
                _autoTestGameReadyStart = false;
                TestReadyToStartGame();
            }
            if (_autoTestGamePausedState)
            {
                _autoTestGamePausedState = false;
                TestPauseGame();
            }
        }


        #region Readying


        void SetLocalPlayerReady()
        {
            if (_currentGameState.Value != GameState.WaitingForPlayers)
                return;
            if (_isLocalPlayerReady)
                return;

            _isLocalPlayerReady = true;
            OnLocalPlayerReady?.Invoke();
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            TestReadyToStartGame();
        }

        void TestReadyToStartGame()
        {
            bool allPlayersReady = true;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                _currentGameState.Value = GameState.StartingGame;
                Debug.Log("Game starting");
            }
        }

        void TestPlayersReadyOnPlayerDisconnect(NetworkManager manager, ConnectionEventData data)
        {
            if (_currentGameState.Value != GameState.WaitingForPlayers)
                return;

            if (data.EventType == ConnectionEvent.ClientDisconnected)
                _autoTestGameReadyStart = true;
        }


        #endregion


        #region Pausing


        void OnGamePausedChange(bool previousValue, bool newValue)
        {
            if (_isGamePaused.Value)
            {
                OnMultiplayerGamePause?.Invoke();
                Time.timeScale = 0f;
            }
            else
            {
                OnMultiplayerGameUnpause?.Invoke();
                Time.timeScale = 1f;
            }
        }

        public void TogglePauseGame()
        {
            if (_currentGameState.Value != GameState.GameStarted)
                return;

            _isLocalPlayerPaused = !_isLocalPlayerPaused;
            if (_isLocalPlayerPaused)
            {
                PauseGameServerRpc();
                OnLocalGamePause?.Invoke();
            }
            else
            {
                UnpauseGameServerRpc();
                OnLocalGameUnpause?.Invoke();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

            TestPauseGame();
        }

        [ServerRpc(RequireOwnership = false)]
        void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

            TestPauseGame();
        }

        void TestPauseGame()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (_playerPausedDictionary.ContainsKey(clientId) && _playerPausedDictionary[clientId])
                {
                    _isGamePaused.Value = true;
                    return;
                }
            }

            _isGamePaused.Value = false;
        }

        void TestPauseOnPlayerDisconnect(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType == ConnectionEvent.ClientDisconnected)
                _autoTestGamePausedState = true;
        }


        #endregion


        void OnStateChange(GameState previousValue, GameState newValue)
        {
            if (GameEnded)
                EndGame();

            OnGameStateChange?.Invoke();
        }

        void EndGame()
        {
            CalculateGrade();
        }

        void CalculateGrade()
        {
            if (!IsServer)
                return;

            if (Bank.Instance.CurrentBalance >= _currentLevelPreset.threeStarsMinimum)
            {
                _finalGrade.Value = Grade.ThreeStars;
            }
            else if (Bank.Instance.CurrentBalance >= _currentLevelPreset.twoStarsMinimum)
            {
                _finalGrade.Value = Grade.TwoStars;
            }
            else if (Bank.Instance.CurrentBalance >= _currentLevelPreset.oneStarMinimum)
            {
                _finalGrade.Value = Grade.OneStar;
            }
            else
            {
                _gameFailed.Value = true;
                _finalGrade.Value = Grade.NoStars;
            }
        }

        public void FailGame()
        {
            _gameFailed.Value = true;

            _currentGameState.Value = GameState.GameEnded;
        }

        public async void ReturnToLobby()
        {
            //Save progress on returning lobby if completed level
            SaveGame();

            //Host and online
            if (GameLobby.Instance.InLobby && IsServer)
            {
                Loader.LoadSceneNetwork(Loader.Scene.SafeHouseScene);

                if (IsServer)
                    GameLobby.Instance.UnlockLobby();
            }
            //Local or client
            else 
            {
                try
                {
                    await MultiplayerManager.Instance.LeaveMultiplayer();
                    Loader.LoadScene(Loader.Scene.SafeHouseScene);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error trying to leave multiplayer" + "\n" + e);
                }
            }
        }

        public void LeaveTeamToLobby()
        {
            //Save progress on returning lobby if completed level
            if (GameEnded && !_gameFailed.Value)
            {
                PersistentDataManager.Instance.OnSave += PersistantDataManager_OnSave;
                RequestSaveServerRpc();
            }
            else
            {
                PersistantDataManager_OnSave();
            }
        }
        async void PersistantDataManager_OnSave()
        {
            try
            {
                await MultiplayerManager.Instance.LeaveMultiplayer();
                Loader.LoadScene(Loader.Scene.SafeHouseScene);
            }
            catch (Exception e)
            {
                Debug.LogError("Error trying to leave multiplayer" + "\n" + e);
            }

            PersistentDataManager.Instance.OnSave -= PersistantDataManager_OnSave;
        }

        public async void ReturnToMenu()
        {
            //Save progress on returning lobby if completed level
            SaveGame();

            try
            {
                await MultiplayerManager.Instance.LeaveMultiplayer();
                Loader.LoadScene(Loader.Scene.MainMenuScene);
            }
            catch (Exception e)
            {
                Debug.LogError("Error trying to leave multiplayer" + "\n" + e);
            }
        }

        public void RestartGame()
        {
            //Save progress on returning lobby if completed level
            SaveGame();

            Loader.LoadLevel(LevelSelectManager.Instance.CurrentLevel);
        }

        void SaveGame()
        {
            if (GameEnded && !_gameFailed.Value)
            {
                SaveGameClientRpc(Bank.Instance.CurrentBalance, _finalGrade.Value);
            }
        }

        //For players that leave team to lobby
        [ServerRpc(RequireOwnership = false)]
        void RequestSaveServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            
            SaveGameClientRpc(Bank.Instance.CurrentBalance, _finalGrade.Value, clientRpcParams);
        }

        [ClientRpc]
        void SaveGameClientRpc(int currentScore, Grade finalGrade, ClientRpcParams clientRpcParams = default)
        {
            LevelSelectManager.Instance.CompleteCurrentLevel(currentScore, finalGrade);
            PersistentDataManager.Instance.SaveGame();
        }
    }
}
