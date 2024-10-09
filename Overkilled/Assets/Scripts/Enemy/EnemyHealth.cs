using System;
using UnityEngine;

public class EnemyHealth : ObjectHealth, IStunnable
{
    [Tooltip("The time that this enemy is stunned for when hit")]
    [SerializeField] float _staggerTime;
    [Tooltip("If this target can be stunned")]
    [SerializeField] bool _canBeStunned = true;
    [Tooltip("If this target can be flattened")]
    [SerializeField] bool _canBeFlattened = true;

    public delegate void EnemyDamageAction(float value);
    public event EnemyDamageAction OnDamage;
    public event EnemyDamageAction OnStun;
    public event EnemyDamageAction OnFlatten;

    public float GetStaggerTime() { return _staggerTime; }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnDamage?.Invoke(damage);
    }

    public void Stun(float time, bool flatten)
    {
        if (!_canBeStunned)
            return;

        OnStun?.Invoke(time);

        if (_canBeFlattened && flatten)
            OnFlatten?.Invoke(time);
    }

    public override void Die()
    {
        base.Die();
    }
}
