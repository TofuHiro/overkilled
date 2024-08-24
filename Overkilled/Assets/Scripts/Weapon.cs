using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponSO _weaponSO;

    protected bool _isAttacking = false, _isSecondaryAttacking;
    float _nextTimeToAttack = 0f;
    float _timer = 0f;

    public int Durability
    {
        get { return _durability; } 
        protected set
        {
            _durability = Mathf.Clamp(value, 0, _weaponSO.durability);
        }
    }
    private int _durability;

    public WeaponSO GetWeaponInfo() { return _weaponSO; }

    void Start()
    {
        SetNextTimeAttack(_weaponSO.attackFrequency);
        Durability = _weaponSO.durability;
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

    protected virtual void Attack()
    {
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
