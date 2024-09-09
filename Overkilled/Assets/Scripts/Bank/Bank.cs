using System;

public static class Bank
{
    public static event Action OnBalanceChange;
    
    public static int Balance { get; private set; }

    public static void ResetStaticData()
    {
        OnBalanceChange = null;
        ResetBalance();
    }

    public static void ResetBalance()
    {
        Balance = 0;
        OnBalanceChange?.Invoke();
    }

    /// <summary>
    /// Set the shared balance to a number
    /// </summary>
    /// <param name="number"></param>
    public static void SetMoney(int number)
    {
        Balance = number;
        OnBalanceChange?.Invoke();
    }

    /// <summary>
    /// Add an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public static void AddMoney(int amount) 
    { 
        Balance += amount;
        OnBalanceChange?.Invoke();
    }

    /// <summary>
    /// Remove an amount to the current balance
    /// </summary>
    /// <param name="amount"></param>
    public static void RemoveMoney(int amount) 
    { 
        Balance -= amount;
        OnBalanceChange?.Invoke();
    }
}
