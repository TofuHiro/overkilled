using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Tooltip("The center point to check for attack collisions")]
    [SerializeField] Transform _swingPoint;

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
            Swing(_meleeSO.heavyAreaOfAttack, _meleeSO.heavyDamage, _meleeSO.heavyKnockbackForce, 2);
            SetNextTimeAttack(_meleeSO.heavyAttackFrequency);
        }
        else
        {
            //Normal Attack
            Swing(_meleeSO.areaOfAttack, _meleeSO.damage, _meleeSO.knockbackForce, 1);
            //Next time to attack is set in base class
        }
    }

    void Swing(Vector3 areaOfAttack, float damage, float knockbackForce, int durabilityConsumption)
    {
        Collider[] colliders = Physics.OverlapBox(_swingPoint.position, areaOfAttack / 2f);

        bool hit = false;
        foreach (Collider collider in colliders)
        {
            //Damage
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                CombatManager.Instance.DamageTarget(damagable, damage);
                hit = true;
            }

            //Impact Force
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddHitForce(rb, _swingPoint.forward * knockbackForce, collider.ClosestPoint(transform.position));
        }

        //Only reduce durability if damagable hit
        if (hit)
            DecreaseDurablity(durabilityConsumption);
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);
        //
    }

    void OnDrawGizmos()
    {
        if (_meleeSO == null)
            _meleeSO = (MeleeSO)_weaponSO;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_swingPoint.position, _meleeSO.areaOfAttack);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_swingPoint.position, _meleeSO.heavyAreaOfAttack);
    }
}
