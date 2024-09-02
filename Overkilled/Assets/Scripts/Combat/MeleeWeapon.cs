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

    protected override void Attack()
    {
        base.Attack();
        if (Durability <= 0)
            return;

        if (_isSecondaryAttacking)
        {
            HeavySwing();
            SetNextTimeAttack(_meleeSO.heavyAttackFrequency);
        }
        else
        {
            Swing();
        }
    }

    void Swing()
    {
        Collider[] colliders = Physics.OverlapBox(_swingPoint.position, _meleeSO.areaOfAttack/2f);

        bool hit = false;
        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                CombatManager.Instance.DamageTarget(damagable, _meleeSO.damage);
                hit = true;
            }

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddHitForce(rb, _swingPoint.forward * _meleeSO.knockbackForce, collider.ClosestPoint(transform.position));
        }

        if (hit)
            DecreaseDurablity(1);
    }

    void HeavySwing()
    {
        Collider[] colliders = Physics.OverlapBox(_swingPoint.position, _meleeSO.heavyAreaOfAttack / 2f);

        bool hit = false;
        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                CombatManager.Instance.DamageTarget(damagable, _meleeSO.heavyDamage);
                hit = true;
            }

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddHitForce(rb, _swingPoint.forward * _meleeSO.heavyKnockbackForce, collider.ClosestPoint(transform.position));
        }

        if (hit)
        {
            DecreaseDurablity(2);
        }
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);

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
