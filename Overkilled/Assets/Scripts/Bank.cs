using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance;

    public event Action OnBalanceChange;
    
    public int Balance { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Bank found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    void Start()
    {
        SetMoney(0);
    }

    /// <summary>
    /// Set the shared balance to a number
    /// </summary>
    /// <param name="number"></param>
    public void SetMoney(int number)
    {
        Balance = number;
        OnBalanceChange?.Invoke();
    }

    /// <summary>
    /// Add an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount) 
    { 
        Balance += amount;
        OnBalanceChange?.Invoke();
    }

    /// <summary>
    /// Remove an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveMoney(int amount) 
    { 
        Balance -= amount;
        OnBalanceChange?.Invoke();
    }
}
