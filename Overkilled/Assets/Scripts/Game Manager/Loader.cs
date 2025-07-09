using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Loader: MonoBehaviour 
{
    [SerializeField] Animator _animator;
    [SerializeField] Transform _raycastBlocker;

    public enum Scene
    {
        MainMenuScene,
        SafeHouseScene,
        LoadingScene,
        BaseLevelScene,
    }

    public enum TransitionType
    {
        None,
        FadeIn, 
        FadeOut,
        Constant,
    }

    public static Loader Instance;

    float _transitionTime = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of SceneTransitioner found. Destroying " + name);
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Loads a scene locally with transition
    /// </summary>
    /// <param name="targetScene"></param>
    /// <param name="transitionOut">The transition type to use when exiting the current scene</param>
    /// <param name="transitionIn">The transition type to use when entering the target scene</param>
    public void LoadScene(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        EventSystem.current.enabled = false;
        StartCoroutine(Load(targetScene.ToString(), transitionOut, transitionIn));
    }
    IEnumerator Load(string scene, TransitionType transitionOut, TransitionType transitionIn)
    {
        _animator.SetTrigger(transitionOut.ToString());
        yield return new WaitForSeconds(_transitionTime);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.LoadSceneAsync(scene);

        void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            _animator.SetTrigger(transitionIn.ToString());
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
    }



    /// <summary>
    /// Load a scene to all connected players in the server
    /// </summary>
    /// <param name="targetScene"></param>
    public void LoadSceneNetwork(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadSceneNetworkServerRpc(targetScene, transitionOut, transitionIn);
    }

    [ServerRpc]
    void LoadSceneNetworkServerRpc(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadSceneNetworkClientRpc(targetScene, transitionOut, transitionIn);
    }

    [ClientRpc]
    void LoadSceneNetworkClientRpc(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        EventSystem.current.enabled = false;
        StartCoroutine(LoadNetwork(targetScene.ToString(), transitionOut, transitionIn));
    }

    IEnumerator LoadNetwork(string scene, TransitionType transitionOut, TransitionType transitionIn)
    {
        _animator.SetTrigger(transitionOut.ToString());
        yield return new WaitForSeconds(_transitionTime);

        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);

        void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            _animator.SetTrigger(transitionIn.ToString());
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
        }
    }



    /// <summary>
    /// Loads a given level 
    /// </summary>
    /// <param name="level"></param>
    public void LoadLevel(Level level, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadLevelServerRpc(level, transitionOut, transitionIn);
    }

    [ServerRpc]
    void LoadLevelServerRpc(Level level, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadLevelClientRpc(level, transitionOut, transitionIn);
    }

    [ClientRpc]
    void LoadLevelClientRpc(Level level, TransitionType transitionOut, TransitionType transitionIn)
    {
        EventSystem.current.enabled = false;
        StartCoroutine(LoadLevel(level.ToString(), transitionOut, transitionIn));
    }

    IEnumerator LoadLevel(string scene, TransitionType transitionOut, TransitionType transitionIn)
    {
        _animator.SetTrigger(transitionOut.ToString());    
        yield return new WaitForSeconds(_transitionTime);
        
        // Loads the 'base level scene' that stores the base info for a level. Level loaded first for light maps
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLevelLoadComplete;
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);

        void SceneManager_OnLevelLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            Debug.Log("E ");
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnLevelLoadComplete;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnBaseLoadComplete;

            NetworkManager.Singleton.SceneManager.LoadScene(Scene.BaseLevelScene.ToString(), LoadSceneMode.Additive);

            void SceneManager_OnBaseLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
            {
                _animator.SetTrigger(transitionIn.ToString());
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnBaseLoadComplete;
            }
        }
    }
}
