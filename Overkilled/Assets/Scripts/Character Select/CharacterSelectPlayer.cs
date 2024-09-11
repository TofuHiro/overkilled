using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] int _playerIndex;
    [SerializeField] GameObject _readyObject;
    [SerializeField] PlayerVisuals _playerVisuals;
    [SerializeField] Button _kickButton;

    void Awake()
    {
        _kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
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
