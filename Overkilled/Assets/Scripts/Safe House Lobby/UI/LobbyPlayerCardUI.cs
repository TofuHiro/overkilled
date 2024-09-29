using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCardUI : MonoBehaviour
{
    [Tooltip("The index order of this character select player in the scene")]
    [SerializeField] int _playerIndex;
    [Tooltip("The player name text")]
    [SerializeField] TMP_Text _playerNameText;
    [Tooltip("The ready text to show")]
    [SerializeField] TMP_Text _readyText;
    [Tooltip("The kick button to kick this character player")]
    [SerializeField] Button _kickButton;

    void Awake()
    {
        _kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChange += UpdatePlayer;
        PlayerReadyManager.Instance.OnPlayerReadyChange += UpdatePlayer;

        UpdatePlayer();
    }

    void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChange -= UpdatePlayer;
    }

    void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

            _readyText.text = PlayerReadyManager.Instance.IsPlayerReady(playerData.clientId) ? "Ready" : "Not Ready";
            _playerNameText.text = playerData.playerName.ToString();

            //Server only + non host
            _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && _playerIndex > 0);

            Show();
        }
        else
        {
            Hide();
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
