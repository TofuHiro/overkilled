using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button _playButton;
    [SerializeField] Button _quitButton;

    void Awake()
    {
        _playButton.onClick.AddListener(LoadToSafeHouse);

        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    void LoadToSafeHouse()
    {
        Loader.Instance.LoadScene(Loader.Scene.LoadingScene, Loader.TransitionType.FadeOut, Loader.TransitionType.Constant);
    }
}
