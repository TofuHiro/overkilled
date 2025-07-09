using System;
using UnityEngine;

public class EnemyHealth : ObjectHealth, IStunnable
{
    [Tooltip("The time that this enemy is stunned for when hit")]
    [SerializeField] float _staggerTime;
    [Tooltip("If this target can be staggered. Staggered is a light stun state which can interupt this enemy's actions")]
    [SerializeField] bool _canBeStaggered = true;
    [Tooltip("If this target can be stunned. Stunned is a state where the enemy is stopped for an amount of time")]
    [SerializeField] bool _canBeStunned = true;
    [Tooltip("If this target can be flattened. Flattened is a stun variant that affect this enemy visually. The enemy, when flattened, will seem squashed and aren't able to do anything")]
    [SerializeField] bool _canBeFlattened = true;

    public delegate void EnemyDamageAction(float value);
    public event EnemyDamageAction OnDamage;
    public event EnemyDamageAction OnStun;
    public event EnemyDamageAction OnFlatten;
    public static event Action OnDeath;

    public float GetStaggerTime() { return _staggerTime; }
    public bool CanBeStaggered { get { return _canBeStaggered; } }
    public bool CanBeStunned { get { return _canBeStunned; } }

    public override void TakeDamage(float damage)
    {
        if (!IsServer)
            return;

        base.TakeDamage(damage);

        OnDamage?.Invoke(damage);
    }

    public void Stun(float time, bool flatten)
    {
        if (!IsServer)
            return;
        if (!_canBeStunned)
            return;

        OnStun?.Invoke(time);

        if (_canBeFlattened && flatten)
            OnFlatten?.Invoke(time);
    }

    public override void Die()
    {
        if (!IsServer)
            return;

        base.Die();

        OnDeath?.Invoke();
    }
}
