using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SurvivalGame
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] LevelPreset _levelPreset;
        [Tooltip("The count down timer when the game is starting")]
        [SerializeField] float _gameStartCountdownTime = 3f;

        public static GameManager Instance { get; private set; }

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

        /// <summary>
        /// The final grade given after the game has ended
        /// </summary>
        public Grade LevelGrade { get; private set; }

        public delegate void GameInitialize(LevelPreset preset);
        public static event GameInitialize OnGameInitialize;
        public static event Action OnGameStateChange;
        public static event Action OnLocalPlayerReady;
        public static event Action OnLocalGamePause;
        public static event Action OnLocalGameUnpause;
        public static event Action OnMultiplayerGamePause;
        public static event Action OnMultiplayerGameUnpause;

        Dictionary<ulong, bool> _playerReadyDictionary;
        Dictionary<ulong, bool> _playerPausedDictionary;

        NetworkVariable<GameState> _currentGameState = new NetworkVariable<GameState>();
        NetworkVariable<float> _countdownTimer = new NetworkVariable<float>();
        NetworkVariable<float> _gameTimer = new NetworkVariable<float>();
        NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);

        bool _isLocalPlayerReady = false, _isLocalPlayerPaused = false;
        bool _autoTestGamePausedState, _autoTestGameReadyStart;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
                Destroy(Instance);
            }

            Instance = this;

            _playerReadyDictionary = new Dictionary<ulong, bool>();
            _playerPausedDictionary = new Dictionary<ulong, bool>();

            OnGameStateChange += CalculateGrade;
        }

        public override void OnNetworkSpawn()
        {
            InitializeLevel();
            _currentGameState.OnValueChanged += OnStateChange;
            _isGamePaused.OnValueChanged += OnGamePausedChange;

            NetworkManager.Singleton.OnConnectionEvent += TestPauseOnPlayerDisconnect;
            NetworkManager.Singleton.OnConnectionEvent += TestPlayersReadyOnPlayerDisconnect;
        }

        public override void OnNetworkDespawn()
        {
            _currentGameState.OnValueChanged -= OnStateChange;
            _isGamePaused.OnValueChanged -= OnGamePausedChange;

            NetworkManager.Singleton.OnConnectionEvent -= TestPauseOnPlayerDisconnect;
            NetworkManager.Singleton.OnConnectionEvent -= TestPlayersReadyOnPlayerDisconnect;
        }

        void Start()
        {
            PlayerController.OnPlayerInteractInput += SetLocalPlayerReady;
            PlayerController.OnPlayerPauseInput += TogglePauseGame;
        }

        void OnDisable()
        {
            PlayerController.OnPlayerInteractInput -= SetLocalPlayerReady;
            PlayerController.OnPlayerPauseInput -= TogglePauseGame;
        }
    
        void TestPauseOnPlayerDisconnect(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType == ConnectionEvent.ClientDisconnected)
                _autoTestGamePausedState = true;
        }

        void TestPlayersReadyOnPlayerDisconnect(NetworkManager manager, ConnectionEventData data)
        {
            if (_currentGameState.Value != GameState.WaitingForPlayers)
                return;

            if (data.EventType == ConnectionEvent.ClientDisconnected)
                _autoTestGameReadyStart = true;
        }

        void OnStateChange(GameState previousValue, GameState newValue)
        {
            OnGameStateChange?.Invoke();
        }

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

        void InitializeLevel()
        {
            Bank.ResetBalance();
            _currentGameState.Value = GameState.WaitingForPlayers;
            _countdownTimer.Value = _gameStartCountdownTime;
            _gameTimer.Value = _levelPreset.timeLimit;
            OnGameInitialize?.Invoke(_levelPreset);
        }

        void SetLocalPlayerReady()
        {
            if (_currentGameState.Value != GameState.WaitingForPlayers && _isLocalPlayerReady)
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

        void CalculateGrade()
        {
            if (!GameEnded) return;

            if (Bank.Balance >= _levelPreset.fiveStarsMinimum)
                LevelGrade = Grade.FiveStars;
            else if (Bank.Balance >= _levelPreset.fourStarsMinimum)
                LevelGrade = Grade.FourStars;
            else if (Bank.Balance >= _levelPreset.threeStarsMinimum)
                LevelGrade = Grade.ThreeStars;
            else if (Bank.Balance >= _levelPreset.twoStarsMinimum)
                LevelGrade = Grade.TwoStars;
            else if (Bank.Balance >= _levelPreset.oneStarMinimum)
                LevelGrade = Grade.OneStar;
            else
                LevelGrade = Grade.NoStars;
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
    }
}
