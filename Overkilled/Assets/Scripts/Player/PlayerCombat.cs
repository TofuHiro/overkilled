using System;
using UnityEngine;

[RequireComponent(typeof(PlayerHand))]
public class PlayerCombat : MonoBehaviour
{
    [Tooltip("The center point to check for attack collisions")]
    [SerializeField] Transform _swingPoint;
    [Tooltip("The point to fire ray from")]
    [SerializeField] Transform _raycastPoint;
    [Tooltip("The reference of the weapon this entity starts with when spawned")]
    [SerializeField] Weapon _startingWeapon;

    public delegate void IdleAction(IdleType idleType);
    public event IdleAction OnEquip;
    public event Action OnDrop;
    public delegate void AttackAction(AttackType attackType);
    public event AttackAction OnAttack;
    public event AttackAction OnSecondaryAttack;
    public delegate void SecondaryAttackAction(float movementSpeedMultiplier, SecondaryType secondaryType);
    public event SecondaryAttackAction OnSecondaryAttackStart;
    public event SecondaryAttackAction OnSecondaryAttackStop;

    Weapon _currentWeapon;
    PlayerHand _hand;

    void Awake()
    {
        _hand = GetComponent<PlayerHand>();  
        
        if (_startingWeapon)
        {
            _currentWeapon = _startingWeapon;
        }
    }

    void Start()
    {
        _hand.OnPickUp += PlayerHand_OnPickUp;
        _hand.OnDrop += PlayerHand_OnDrop;
    }

    void PlayerHand_OnPickUp(Item item)
    {
        _currentWeapon = item.GetComponent<Weapon>();
        if (_currentWeapon != null)
        {
            SetAttackState(false);
            SetSecondaryAttackState(false);
            _currentWeapon.OnAttack += Weapon_OnAttack;
            _currentWeapon.OnSecondaryAttack += Weapon_OnSecondaryAttack;
            OnEquip?.Invoke(_currentWeapon.GetWeaponInfo().idleType);
        }
    }

    void PlayerHand_OnDrop(Item item)
    {
        if (_currentWeapon != null)
        {
            SetAttackState(false);
            SetSecondaryAttackState(false);
            _currentWeapon.OnAttack -= Weapon_OnAttack;
            _currentWeapon.OnSecondaryAttack -= Weapon_OnSecondaryAttack;
            _currentWeapon = null;
            OnDrop?.Invoke();
        }
    }

    void Weapon_OnAttack()
    {
        OnAttack?.Invoke(_currentWeapon.GetWeaponInfo().attackType);
    }

    void Weapon_OnSecondaryAttack()
    {
        OnSecondaryAttack?.Invoke(_currentWeapon.GetWeaponInfo().secondaryAttackType);
    }

    public void SetAttackState(bool state)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.SetAttackState(state);
        
        if (state)
        {
            if (_currentWeapon.GetType() == typeof(RayWeapon))
                _currentWeapon.SetAttackPosition(_raycastPoint);
            
            if (_currentWeapon.GetType() == typeof(MeleeWeapon))
                _currentWeapon.SetAttackPosition(_swingPoint);
        }
    }

    public void SetSecondaryAttackState(bool state)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.SetSecondaryAttackState(state);

        float movementSpeedMultiplier = _currentWeapon.GetWeaponInfo().secondaryAttackMovementSpeedMultiplier;
        SecondaryType secondaryType = _currentWeapon.GetWeaponInfo().secondaryType;

        if (state)
            OnSecondaryAttackStart?.Invoke(movementSpeedMultiplier, secondaryType);
        else
            OnSecondaryAttackStop?.Invoke(movementSpeedMultiplier, secondaryType);
    }

    void OnDrawGizmos()
    {
        if (_raycastPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_raycastPoint.position, transform.forward);
        }
        if (_swingPoint)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_swingPoint.position, Vector3.one / 4);
        }
    }
}
