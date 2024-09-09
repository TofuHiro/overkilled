using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button _createGameButton;
    [SerializeField] Button _joinGameButton;

    void Awake()
    {
        _createGameButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectScene);
        });

        _joinGameButton.onClick.AddListener(() => 
        {
            MultiplayerManager.Instance.StartClient(); 
        });
    }
}
