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

    public void SetMoney(int number)
    {
        Balance = number;
        OnBalanceChange?.Invoke();
    }

    public void AddMoney(int amount) 
    { 
        Balance += amount;
        OnBalanceChange?.Invoke();
    }
    public void RemoveMoney(int amount) 
    { 
        Balance -= amount;
        OnBalanceChange?.Invoke();
    }
}
