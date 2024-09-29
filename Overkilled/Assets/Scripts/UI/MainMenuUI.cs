using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button _playButton;
    [SerializeField] Button _quitButton;

    void Awake()
    {
        _playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(Loader.Scene.SafeHouseScene.ToString());
        });

        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
