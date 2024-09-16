using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerList
{
    public static event Action OnPlayerListUpdate;

    static List<GameObject> s_playerList = new List<GameObject>();

    public static void AddPlayer(GameObject player)
    {
        s_playerList.Add(player);
        OnPlayerListUpdate?.Invoke();
    }

    public static void RemovePlayer(GameObject player) 
    { 
        s_playerList.Remove(player);
        OnPlayerListUpdate?.Invoke();
    }

    public static GameObject[] GetPlayers() 
    { 
        return s_playerList.ToArray(); 
    }

    public static void ResetStaticData() 
    { 
        s_playerList.Clear();
        OnPlayerListUpdate = null;
    }
}
