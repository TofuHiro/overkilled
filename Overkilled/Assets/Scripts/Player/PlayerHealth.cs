using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : ObjectHealth, IInteractable
{
    public delegate void PlayerLifeAction(PlayerHealth player);
    public static event PlayerLifeAction OnPlayerDowned;
    public static event PlayerLifeAction OnPlayerRevive;

    bool _isDown;

    PlayerController _controller;
    PlayerRespawnManager _respawnManager;

    protected override void Start()
    {
        base.Start();

        _controller = GetComponent<PlayerController>();
        _respawnManager = PlayerRespawnManager.Instance;
    }

    public override void Die()
    {
        if (_isDown)
            return;

        _isDown = true;

        _controller.SetCanMove(false);
        OnPlayerDowned?.Invoke(this);
    }

    public void Interact(PlayerInteraction player)
    {
        if (!_isDown)
            return;

        _respawnManager.ReviveTick(this);
    }

    public void RevivePlayer()
    {
        _isDown = false;

        SetHealth(_maxHealth);

        _controller.SetCanMove(true);
        OnPlayerRevive?.Invoke(this);
    }
}
