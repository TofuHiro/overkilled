using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DontDestroyOnLoadCleanUp : MonoBehaviour
{
    void Awake()
    {
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        if (MultiplayerManager.Instance != null)
            Destroy(MultiplayerManager.Instance.gameObject);

        if (GameLobby.Instance != null)
            Destroy(GameLobby.Instance.gameObject);

        if (PersistentDataManager.Instance != null)
            Destroy(PersistentDataManager.Instance.gameObject);

        if (LevelSelectManager.Instance != null)
            Destroy(LevelSelectManager.Instance.gameObject);
    }
}
