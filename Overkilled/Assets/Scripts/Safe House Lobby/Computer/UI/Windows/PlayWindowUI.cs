using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayWindowUI : ComputerWindowUI
{
    [SerializeField] LobbyCreateUI _lobbyCreate;
    [SerializeField] SoloPlayMenuUI _soloSubmenu;
    [SerializeField] GameObject _selectMenu;

    [SerializeField] Button _soloPlayButton;
    [SerializeField] Button _createPublicLobbyButton;
    [SerializeField] Button _createPrivateLobbyButton;

    bool _isSubMenuActive = false;

    protected override void Awake()
    {
        base.Awake();

        _soloPlayButton.onClick.AddListener(OpenSoloPlay);
        _createPrivateLobbyButton.onClick.AddListener(OpenCreatePrivateLobby);
        _createPublicLobbyButton.onClick.AddListener(OpenCreatePublicLobby);
    }

    void Start()
    {
        GameLobby.Instance.OnCreateLobbySuccess += GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess += GameLobby_OnLobbySuccess;

        _lobbyCreate.Hide();
        Hide();
    }

    void OnDestroy()
    {
        GameLobby.Instance.OnCreateLobbySuccess -= GameLobby_OnLobbySuccess;
        GameLobby.Instance.OnJoinSuccess -= GameLobby_OnLobbySuccess;
    }

    void GameLobby_OnLobbySuccess()
    {
        Close();
    }

    void OpenSoloPlay()
    {
        _selectMenu.SetActive(false);
        _isSubMenuActive = true;
        _soloSubmenu.Show();
    }

    void OpenCreatePrivateLobby()
    {
        _selectMenu.SetActive(false);
        _isSubMenuActive = true;
        _lobbyCreate.SetPrivate(true);
        _lobbyCreate.Show();
    }

    void OpenCreatePublicLobby()
    {
        _selectMenu.SetActive(false);
        _isSubMenuActive = true;
        _lobbyCreate.SetPrivate(false);
        _lobbyCreate.Show();
    }

    public override void Back()
    {
        if (_isSubMenuActive)
        {
            _selectMenu.SetActive(true);
            _isSubMenuActive = false;
            _lobbyCreate.Hide();
            _soloSubmenu.Hide();
        }
        else
        {
            Close();
        }
    }
}
