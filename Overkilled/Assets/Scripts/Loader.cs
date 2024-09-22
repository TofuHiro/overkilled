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
        GameScene,
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
}
