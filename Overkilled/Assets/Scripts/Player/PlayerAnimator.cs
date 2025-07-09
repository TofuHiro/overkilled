using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : NetworkBehaviour
{
    const string IS_WALKING = "IsWalking";
    const string IS_RUNNING = "IsRunning";
    const string IS_SECONDARY_ATTACKING = "IsSecondaryAttacking";
    const string IDLE_TYPE = "IdleType";
    const string IS_TWOHANDED_SWING_CHARGING = "IsTwoHandedSwingCharging";

    Animator _animator;

    PlayerHealth _playerHealth;
    PlayerMotor _playerMotor;
    PlayerCombat _playerCombat;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        _playerHealth = GetComponent<PlayerHealth>();
        _playerMotor = GetComponent<PlayerMotor>();
        _playerCombat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        _playerCombat.OnEquip += PlayerCombat_OnEquip;
        _playerCombat.OnDrop += PlayerCombat_OnDrop;
        _playerCombat.OnAttack += PlayerCombat_OnAttack;
        _playerCombat.OnSecondaryAttack += PlayerCombat_OnSecondaryAttack;
        _playerCombat.OnSecondaryAttackStart += PlayerHand_OnSecondaryAttackStart;
        _playerCombat.OnSecondaryAttackStop += PlayerHand_OnSecondaryAttackStop;
    }

    void PlayerCombat_OnEquip(IdleType idleType)
    {
        _animator.SetInteger(IDLE_TYPE, (int)idleType);
    }

    void PlayerCombat_OnDrop()
    {
        _animator.SetInteger(IDLE_TYPE, 0);
    }

    void PlayerCombat_OnAttack(AttackType attackType)
    {
        _animator.SetTrigger(attackType.ToString());
    }

    void PlayerCombat_OnSecondaryAttack(AttackType attackType)
    {
        _animator.SetTrigger(attackType.ToString());
    }

    void PlayerHand_OnSecondaryAttackStart(float movementSpeedMultiplier, SecondaryType secondaryType)
    {
        switch (secondaryType)
        {
            case SecondaryType.None:
                break;
            case SecondaryType.SwingCharge:
                _animator.SetBool(IS_TWOHANDED_SWING_CHARGING, true);
                break;
            case SecondaryType.SlamCharge:
                break;
            case SecondaryType.ADS:
                break;
            default:
                break;
        }
       
        _animator.SetBool(IS_SECONDARY_ATTACKING, true);
    }

    void PlayerHand_OnSecondaryAttackStop(float movementSpeedMultiplier, SecondaryType secondaryType)
    {
        switch (secondaryType)
        {
            case SecondaryType.None:
                break;
            case SecondaryType.SwingCharge:
                _animator.SetBool(IS_TWOHANDED_SWING_CHARGING, false);
                break;
            case SecondaryType.SlamCharge:
                break;
            case SecondaryType.ADS:
                break;
            default:
                break;
        }

        _animator.SetBool(IS_SECONDARY_ATTACKING, false);
    }

    void Update()
    {
        if (!IsOwner)
            return;

        //Down

        _animator.SetBool(IS_WALKING, _playerMotor.IsWalking);
        _animator.SetBool(IS_RUNNING, _playerMotor.IsSprinting);
    }
}
