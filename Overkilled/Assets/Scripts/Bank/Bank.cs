using System;
using Unity.Netcode;
using UnityEngine;

public class Bank : NetworkBehaviour
{
    public static Bank Instance { get; private set; }
    public int CurrentBalance { get; private set; }
    
    public event Action<int> OnBalanceChange;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Bank found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    /// <summary>
    /// Resets the current balance to 0
    /// </summary>
    public void ResetBalance()
    {
        ResetBalanceServerRpc();
    }

    [ServerRpc]
    void ResetBalanceServerRpc()
    {
        CurrentBalance = 0;
        OnBalanceChangeClientRpc(CurrentBalance);
    }

    /// <summary>
    /// Set the current balance to a value
    /// </summary>
    /// <param name="number"></param>
    public void SetMoney(int number)
    {
        SetMoneyServerRpc(number);
    }

    [ServerRpc]
    void SetMoneyServerRpc(int number)
    {
        CurrentBalance = number;
        OnBalanceChangeClientRpc(CurrentBalance);
    }

    /// <summary>
    /// Add an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount) 
    {
        AddMoneyServerRpc(amount);
    }

    [ServerRpc]
    void AddMoneyServerRpc(int amount)
    {
        CurrentBalance += amount;
        OnBalanceChangeClientRpc(CurrentBalance);
    }

    /// <summary>
    /// Remove an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveMoney(int amount) 
    {
        RemoveMoneyServerRpc(amount);
    }

    [ServerRpc]
    void RemoveMoneyServerRpc(int amount)
    {
        CurrentBalance += amount;
        OnBalanceChangeClientRpc(CurrentBalance);
    }

    [ClientRpc]
    void OnBalanceChangeClientRpc(int currentBalance)
    {
        OnBalanceChange?.Invoke(currentBalance);
    }
}
