using SurvivalGame;
using System;
using Unity.Netcode;
using UnityEngine;

public class Bank : NetworkBehaviour
{
    public static Bank Instance { get; private set; }

    public delegate void BankChangeAction(int prevValue, int newValue);
    public BankChangeAction OnBalanceChange;
    
    public int CurrentBalance { 
        get 
        { 
            return _currentBalance.Value; 
        } 
        private set 
        {
            _currentBalance.Value = value;
        }
    }

    NetworkVariable<int> _currentBalance = new NetworkVariable<int>();
    
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

    void Start()
    {
        GameManager.Instance.OnGameInitialize += GameManager_OnGameInitialize;    
    }

    public override void OnNetworkSpawn()
    {
        _currentBalance.OnValueChanged += Balance_OnValueChanged;
    }

    void GameManager_OnGameInitialize(LevelPreset preset)
    {
        ResetBalance();
    }

    void Balance_OnValueChanged(int previousValue, int newValue)
    {
        OnBalanceChange?.Invoke(previousValue, newValue);
    }

    /// <summary>
    /// Resets the current balance to 0
    /// </summary>
    public void ResetBalance()
    {
        if (IsServer)
            ResetBalanceServerRpc();
    }

    [Rpc(SendTo.Server)]
    void ResetBalanceServerRpc()
    {
        CurrentBalance = 0;
    }

    /// <summary>
    /// Set the current balance to a value
    /// </summary>
    /// <param name="number"></param>
    public void SetMoney(int number)
    {
        if (IsServer)
            SetMoneyServerRpc(number);
    }

    [Rpc(SendTo.Server)]
    void SetMoneyServerRpc(int number)
    {
        CurrentBalance = number;
    }

    /// <summary>
    /// Add an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount) 
    {
        if (IsServer)
            AddMoneyServerRpc(amount);
    }

    [Rpc(SendTo.Server)]
    void AddMoneyServerRpc(int amount)
    {
        CurrentBalance += amount;
    }

    /// <summary>
    /// Remove an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveMoney(int amount) 
    {
        if (IsServer)
            RemoveMoneyServerRpc(amount);
    }

    [Rpc(SendTo.Server)]
    void RemoveMoneyServerRpc(int amount)
    {
        CurrentBalance += amount;
    }
}
