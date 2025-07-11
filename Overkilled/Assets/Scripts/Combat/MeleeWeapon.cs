using UnityEngine;

public class MeleeWeapon : Weapon
{
    MeleeSO _meleeSO;

    void Awake()
    {
        _meleeSO = (MeleeSO)_weaponSO;

        if (_meleeSO.GetType() != typeof(MeleeSO))
            Debug.LogWarning("Warning. Incorrect WeaponSO type assigned to weapon " + name);
    }

    public override void Attack()
    {
        base.Attack();
        if (Durability <= 0)
            return;

        if (_isSecondaryAttacking)
        {
            //Heavy Attack
            Collider[] colliders = Swing(_meleeSO.heavyAreaOfAttack, _meleeSO.heavyDamage, _meleeSO.heavyKnockbackForce, 2);
            StunTargets(colliders);
            SetNextTimeAttack(_meleeSO.heavyAttackFrequency);
        }
        else
        {
            //Normal Attack
            Swing(_meleeSO.areaOfAttack, _meleeSO.damage, _meleeSO.knockbackForce, 1);
            //Next time to attack is set in base class
        }
    }

    /// <summary>
    /// Attempt to attack entities in the given area
    /// </summary>
    /// <param name="areaOfAttack">The area to swing at</param>
    /// <param name="damage">The damage to inflict</param>
    /// <param name="knockbackForce">The knockback force to apply to entity hit</param>
    /// <param name="durabilityConsumption">The durability to decrement by with a successful hit</param>
    /// <returns>Returns an array of all colliders hit</returns>
    Collider[] Swing(Vector3 areaOfAttack, float damage, float knockbackForce, int durabilityConsumption)
    {
        Collider[] colliders = Physics.OverlapBox(_attackPosition.position + (_attackPosition.forward * (areaOfAttack.z / 2)), areaOfAttack / 2f, _attackPosition.rotation, ~0, QueryTriggerInteraction.Ignore);

        bool hit = false;
        foreach (Collider collider in colliders)
        {
            //Damage
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                CombatManager.Instance.DamageTarget(damagable, damage, _meleeSO.entityTarget);
                hit = true;
            }

            //Impact Force
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddHitForce(rb, transform.forward * knockbackForce, collider.ClosestPoint(transform.position));
        }

        //Only reduce durability if damagable hit
        if (hit)
            DecreaseDurablity(durabilityConsumption);

        return colliders;
    }

    void StunTargets<T>(T[] targets) where T : Component
    {
        foreach (T target in targets)
        {
            //Stun
            IStunnable stunnable = target.GetComponent<IStunnable>();
            if (stunnable != null)
                CombatManager.Instance.StunTarget(stunnable, _meleeSO.stunTime, _meleeSO.flattenTarget);
        }
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);
        //
    }
}
