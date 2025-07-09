using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : ObjectHealth, IInteractable
{
    public delegate void PlayerLifeAction(PlayerHealth player);
    public static event PlayerLifeAction OnPlayerDowned;
    public static event PlayerLifeAction OnPlayerRevive;

    /// <summary>
    /// Get whether if this player is downed or not
    /// </summary>
    public bool IsDown { 
        get 
        { 
            return _isDown.Value; 
        } 
        private set
        {
            _isDown.Value = value;
        }
    }
    
    NetworkVariable<bool> _isDown = new NetworkVariable<bool>();

    PlayerController _controller;
    PlayerRespawnManager _respawnManager;

    protected override void Start()
    {
        base.Start();

        _controller = GetComponent<PlayerController>();
        _respawnManager = PlayerRespawnManager.Instance;

        _isDown.OnValueChanged += OnDownChanged;
    }

    public override void Die()
    {
        if (IsDown)
            return;

        IsDown = true;
    }

    void OnDownChanged(bool previousValue, bool newValue)
    {
        _controller.SetCanMove(!IsDown);
        _controller.SetCanAttack(!IsDown);
        _controller.SetCanInteract(!IsDown);

        if (IsDown)
        {
            OnPlayerDowned?.Invoke(this);
        }
        else
        {
            OnPlayerRevive?.Invoke(this);
        }
    }

    public void Interact(PlayerInteraction player)
    {
        if (!IsDown)
            return;
        
        _respawnManager.ReviveTick(this);
    }

    public void RevivePlayer()
    {
        if (!IsDown)
            return;

        IsDown = false;

        SetHealth(_maxHealth);
    }
}
