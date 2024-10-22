using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerList
{
    /// <summary>
    /// Invoked when the player list is updated
    /// </summary>
    public static event Action OnPlayerListUpdate;
    public static event Action OnPlayerAliveUpdate;
    public static event Action OnAllPlayersDead;

    static List<GameObject> s_playerList = new List<GameObject>();
    static Dictionary<GameObject, bool> s_playerAliveDictionary = new Dictionary<GameObject, bool>();

    /// <summary>
    /// Add the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void AddPlayer(GameObject player)
    {
        s_playerList.Add(player);
        s_playerAliveDictionary.Add(player, true);
        OnPlayerListUpdate?.Invoke();
    }

    /// <summary>
    /// Remove the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void RemovePlayer(GameObject player) 
    { 
        s_playerList.Remove(player);
        s_playerAliveDictionary.Remove(player);
        OnPlayerListUpdate?.Invoke();
    }

    /// <summary>
    /// Get an array of GameObjects of players
    /// </summary>
    /// <returns>An array of Gameobjects of all the players added</returns>
    public static GameObject[] GetPlayers() 
    { 
        return s_playerList.ToArray(); 
    }

    /// <summary>
    /// Get an array of GameObjects of all players that are currently alive.
    /// </summary>
    /// <returns>An array of Gameobjects of all the players that are alive</returns>
    public static GameObject[] GetAlivePlayers()
    {
        List<GameObject> list = new List<GameObject>();

        foreach (var playerAlive in s_playerAliveDictionary)
            if (playerAlive.Value == true)
                list.Add(playerAlive.Key);

        return list.ToArray();
    }

    /// <summary>
    /// Set whether a given player is alive or not
    /// </summary>
    /// <param name="player">The player GameObject</param>
    /// <param name="isAlive">If this player is alive or not</param>
    public static void SetPlayerAlive(GameObject player, bool isAlive)
    {
        s_playerAliveDictionary[player] = isAlive;
        OnPlayerAliveUpdate?.Invoke();

        bool allPlayersDead = true;
        foreach (var playerAlive in s_playerAliveDictionary)
            if (playerAlive.Value == true)
                allPlayersDead = false;

        if (allPlayersDead)
            OnAllPlayersDead?.Invoke();
    }

    /// <summary>
    /// Resets the player list
    /// </summary>
    public static void ResetStaticData() 
    { 
        s_playerList.Clear();
        s_playerAliveDictionary.Clear();
        OnPlayerListUpdate = null;
        OnPlayerAliveUpdate = null;
    }
}
