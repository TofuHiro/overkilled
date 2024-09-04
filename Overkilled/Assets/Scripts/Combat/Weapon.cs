using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Weapon : NetworkBehaviour
{
    [Tooltip("Ensure that the type of this ScriptableObject matches the type of weapon")]
    [SerializeField] protected WeaponSO _weaponSO;

    protected bool _isAttacking = false, _isSecondaryAttacking;
    float _nextTimeToAttack = 0f;
    float _timer = 0f;

    /// <summary>
    /// Current durability of this weapon
    /// </summary>
    public int Durability { 
        get 
        { 
            return _durability.Value; 
        } 
    }

    NetworkVariable<int> _durability = new NetworkVariable<int>(0);

    public WeaponSO GetWeaponInfo() { return _weaponSO; }

    protected void DecreaseDurablity(int amount)
    {
        DecreaseDurabilityServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    void DecreaseDurabilityServerRpc(int amount)
    {
        _durability.Value = Mathf.Clamp(Durability - amount, 0, _weaponSO.durability);
    }

    void Start()
    {
        SetNextTimeAttack(_weaponSO.attackFrequency);
        _durability.Value = _weaponSO.durability;
    }

    void Update()
    {
        if (_timer < _nextTimeToAttack)
            _timer += Time.deltaTime;

        if (_isAttacking && _timer >= _nextTimeToAttack)
            Attack();
    }

    public void SetAttackState(bool state)
    {
        _isAttacking = state;
    }

    public virtual void Attack()
    {
        if (_timer < _nextTimeToAttack)
            return;

        if (Durability > 0)
        {
            if (_weaponSO.semiAutomatic)
                SetAttackState(false);
            SetNextTimeAttack(_weaponSO.attackFrequency);
        }
        else
        {
            Debug.Log("Cant attack");
            SetAttackState(false);
            SetNextTimeAttack(_weaponSO.attackFrequency);
        }
    }

    public virtual void SetSecondaryAttackState(bool state)
    {
        _isSecondaryAttacking = state;
    }

    public void OnPickup()
    {
        SetAttackState(false);
        SetSecondaryAttackState(false);

        _nextTimeToAttack = _timer + _weaponSO.attackFrequency;
    }

    public void OnDrop()
    {
        SetAttackState(false);
        SetSecondaryAttackState(false);
    }

    protected void SetNextTimeAttack(float delay)
    {
        _nextTimeToAttack = _timer + delay;
    }
}
