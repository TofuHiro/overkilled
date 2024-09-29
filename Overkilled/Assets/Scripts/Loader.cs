using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        SafeHouseScene,
        LoadingScene,
        BaseLevelScene,
    }

    public enum Level
    {
        None, Jan_1, Jan_2, Jan_3
    }

    /// <summary>
    /// Load a scene locally
    /// </summary>
    /// <param name="targetScene"></param>
    public static void LoadScene(Scene targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    /// <summary>
    /// Load a scene to all connected players in the server
    /// </summary>
    /// <param name="targetScene"></param>
    public static void LoadSceneNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadLevel(Level level)
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;

        NetworkManager.Singleton.SceneManager.LoadScene(level.ToString(), LoadSceneMode.Single);
    }

    static void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(Scene.BaseLevelScene.ToString(), LoadSceneMode.Additive);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }
}
