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
        [SerializeField] float _gameStartCountdownTime = 3f;

        public static GameManager Instance { get; private set; }

        public bool GameStarting { get { return _currentGameState.Value == GameState.StartingGame; } }
        public bool GameStarted { get { return _currentGameState.Value == GameState.GameStarted; } }
        public bool GameEnded { get { return _currentGameState.Value == GameState.GameEnded; } }
        public Grade LevelGrade { get; private set; }

        public delegate void GameInitialize(LevelPreset preset);
        public static event GameInitialize OnGameInitialize;
        public static event Action OnGameStateChange;
        public static event Action OnGamePause;
        public static event Action OnGameUnpause;
        public static event Action OnLocalPlayerReady;

        Dictionary<ulong, bool> _playerReadyDictionary;
        NetworkVariable<GameState> _currentGameState = new NetworkVariable<GameState>();
        NetworkVariable<float> _countdownTimer = new NetworkVariable<float>();
        NetworkVariable<float> _gameTimer = new NetworkVariable<float>();
        bool _isLocalPlayerReady = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
                Destroy(Instance);
            }

            Instance = this;

            _playerReadyDictionary = new Dictionary<ulong, bool>();

            OnGameStateChange += CalculateGrade;
        }

        public override void OnNetworkSpawn()
        {
            InitializeLevel();

            _currentGameState.OnValueChanged += OnStateChange;
        }

        void OnStateChange(GameState previousValue, GameState newValue)
        {
            OnGameStateChange?.Invoke();
        }

        void Start()
        {
            PlayerController.OnPlayerInteractInput += SetLocalPlayerReady;
        }

        void OnDisable()
        {
            PlayerController.OnPlayerInteractInput -= SetLocalPlayerReady;
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
                case GameState.GamePaused:
                    break;
                case GameState.GameEnded:
                    break;
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
            bool allPlayersReady = true;
            foreach (NetworkClient networkClient in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!_playerReadyDictionary.ContainsKey(networkClient.ClientId) || !_playerReadyDictionary[networkClient.ClientId])
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
    }
}
