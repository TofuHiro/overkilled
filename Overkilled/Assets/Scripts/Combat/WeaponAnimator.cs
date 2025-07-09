using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponAnimator : MonoBehaviour
{
    const string ATTACK = "Attack";
    const string SECONDARY_HOLD = "IsSecondaryAttacking";

    Animator _animator; 
    Weapon _weapon;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _weapon = GetComponent<Weapon>();    
    }

    void Start()
    {
        _weapon.OnAttack += Weapon_OnAttack;
        _weapon.OnSecondaryAttackStart += Weapon_OnSecondaryAttackStart;
        _weapon.OnSecondaryAttackStop += Weapon_OnSecondaryAttackStop;
    }

    void Weapon_OnAttack()
    {
        _animator.SetTrigger(ATTACK);
    }

    void Weapon_OnSecondaryAttackStart()
    {
       _animator.SetBool(SECONDARY_HOLD, true);
    }

    void Weapon_OnSecondaryAttackStop()
    {
        _animator.SetBool(SECONDARY_HOLD, false);
    }
}
