using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerList
{
    public delegate void PlayerListAction(GameObject player);

    /// <summary>
    /// Invoked when a player is added to the list
    /// </summary>
    public static event PlayerListAction OnPlayerAdd;

    /// <summary>
    /// Invoked when a player is removed from the list
    /// </summary>
    public static event PlayerListAction OnPlayerRemove;

    /// <summary>
    /// Invoked when the player list is updated
    /// </summary>
    public static event Action OnPlayerListChange;

    static List<GameObject> s_playerList = new List<GameObject>();

    /// <summary>
    /// Add the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void AddPlayer(GameObject player)
    {
        s_playerList.Add(player);
        
        OnPlayerAdd?.Invoke(player);
        OnPlayerListChange?.Invoke();
    }

    /// <summary>
    /// Remove the GameObject reference of a player
    /// </summary>
    /// <param name="player"></param>
    public static void RemovePlayer(GameObject player)
    {
        s_playerList.Remove(player);

        OnPlayerRemove?.Invoke(player);
        OnPlayerListChange?.Invoke();
    }

    /// <summary>
    /// Get an array of GameObjects of players
    /// </summary>
    /// <returns>An array of Gameobjects of all the players added</returns>
    public static GameObject[] GetPlayers()
    {
        return s_playerList.ToArray();
    }

    public static int GetPlayerListIndex(GameObject player)
    {
        for (int i = 0; i < s_playerList.Count; i++)
            if (s_playerList[i] = player)
                return i;

        Debug.LogError("Error. Player " + player.name + "not found in player list");
        return -1;
    }

    public static GameObject GetPlayer(int index)
    {
        return s_playerList[index];
    }
 
    /// <summary>
    /// Resets the player list
    /// </summary>
    public static void ResetStaticData()
    {
        s_playerList.Clear();
        OnPlayerAdd = null;
        OnPlayerRemove = null;
    }
}
