using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [Tooltip("The index order of this character select player in the scene")]
    [SerializeField] int _playerIndex;
    [Tooltip("The ready text to show")]
    [SerializeField] GameObject _readyObject;
    [Tooltip("The player visuals to update on this character")]
    [SerializeField] PlayerVisuals _playerVisuals;
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
        MultiplayerManager.OnPlayerDataNetworkListChange += UpdatePlayer;
        CharacterSelectReady.Instance.OnPlayerReadyChange += UpdatePlayer;

        _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && _playerIndex > 0);

        UpdatePlayer();
    }

    void OnDestroy()
    {
        MultiplayerManager.OnPlayerDataNetworkListChange -= UpdatePlayer;
        CharacterSelectReady.Instance.OnPlayerReadyChange -= UpdatePlayer;
    }

    void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            
            _readyObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            _playerVisuals.SetPlayerModel(playerData.PlayerModelId);

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
