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

    static List<GameObject> s_playerList = new List<GameObject>();

    /// <summary>
    /// Add the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void AddPlayer(GameObject player)
    {
        s_playerList.Add(player);
        OnPlayerListUpdate?.Invoke();
    }

    /// <summary>
    /// Remove the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void RemovePlayer(GameObject player) 
    { 
        s_playerList.Remove(player);
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
    /// Resets the player list
    /// </summary>
    public static void ResetStaticData() 
    { 
        s_playerList.Clear();
        OnPlayerListUpdate = null;
    }
}
