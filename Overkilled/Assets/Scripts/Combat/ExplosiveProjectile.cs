using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    ExplosiveProjectileSO _explosiveProjectile;

    float _timer = 0f;

    void Awake()
    {
        _explosiveProjectile = (ExplosiveProjectileSO)_projectileSO;

        if (_explosiveProjectile.GetType() != typeof(ExplosiveProjectileSO))
            Debug.LogWarning("Warning. Incorrect WeaponSO type assigned to weapon " + name);
    }

    protected override void FixedUpdate()
    {
        if (!_explosiveProjectile.explodeOnContact)
            return;

        base.FixedUpdate();
    }

    void Update()
    {
        if (_isFired)
        {
            _timer += Time.deltaTime;
            if (_timer >= _explosiveProjectile.explosionFuse)
            {
                Explode();
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!_explosiveProjectile.explodeOnContact)
            return;

        base.OnCollisionEnter(collision);
    }

    protected override void HitTarget(Transform target, Vector3 hitPoint)
    {
        Explode();
    }

    void Explode()
    {
        Collider[] colliders =  Physics.OverlapSphere(transform.position, _explosiveProjectile.explosionRadius);

        foreach (Collider collider in colliders)
        {
            //Damage
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
                CombatManager.Instance.DamageTarget(damagable, _explosiveProjectile.explosionDamage, _projectileSO.entityTarget);

            //Apply Force
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddExplosiveForce(rb, _explosiveProjectile.explosionForce, transform.position, _explosiveProjectile.explosionRadius);
        }

        MultiplayerManager.Instance.DestroyObject(gameObject);
    }
}
