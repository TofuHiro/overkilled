using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Loader: MonoBehaviour 
{
    [SerializeField] Transform _raycastBlocker;
    
    Animator _animator;

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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Loads a scene locally with transition
    /// </summary>
    /// <param name="targetScene"></param>
    /// <param name="transitionOut">The transition type to use when exiting the current scene</param>
    /// <param name="transitionIn">The transition type to use when entering the target scene</param>
    public void LoadScene(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        if (EventSystem.current) 
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

    [Rpc(SendTo.Server)]
    void LoadSceneNetworkServerRpc(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadSceneNetworkClientRpc(targetScene, transitionOut, transitionIn);
    }

    [Rpc(SendTo.Everyone)]
    void LoadSceneNetworkClientRpc(Scene targetScene, TransitionType transitionOut, TransitionType transitionIn)
    {
        if (EventSystem.current)
            EventSystem.current.enabled = false;

        StartCoroutine(LoadNetwork(targetScene.ToString(), transitionOut, transitionIn));
    }

    IEnumerator LoadNetwork(string scene, TransitionType transitionOut, TransitionType transitionIn)
    {
        _animator.SetTrigger(transitionOut.ToString());

        yield return new WaitForSeconds(_transitionTime);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventComplete;
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);

        void SceneManager_OnLoadEventComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            _animator.SetTrigger(transitionIn.ToString());
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventComplete;
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

    [Rpc(SendTo.Server)]
    void LoadLevelServerRpc(Level level, TransitionType transitionOut, TransitionType transitionIn)
    {
        LoadLevelClientRpc(level, transitionOut, transitionIn);
    }

    [Rpc(SendTo.Everyone)]
    void LoadLevelClientRpc(Level level, TransitionType transitionOut, TransitionType transitionIn)
    {
        if (EventSystem.current)
            EventSystem.current.enabled = false;

        StartCoroutine(LoadLevel(level.ToString(), transitionOut, transitionIn));
    }

    IEnumerator LoadLevel(string scene, TransitionType transitionOut, TransitionType transitionIn)
    {
        _animator.SetTrigger(transitionOut.ToString());   
        
        yield return new WaitForSeconds(_transitionTime);
        
        // Loads the 'base level scene' that stores the base info for a level. Level loaded first for light maps
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLevelLoadEventCompleted;
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);

        void SceneManager_OnLevelLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLevelLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnBaseLoadEventComplete;

            NetworkManager.Singleton.SceneManager.LoadScene(Scene.BaseLevelScene.ToString(), LoadSceneMode.Additive);

            void SceneManager_OnBaseLoadEventComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
            {
                _animator.SetTrigger(transitionIn.ToString());
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnBaseLoadEventComplete;
            }
        }
    }
}
