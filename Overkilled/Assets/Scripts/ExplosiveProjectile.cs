using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    float _explosionDamage;
    float _explosionForce;
    float _explosionRadius;
    bool _explodeOnContact;
    float _explosionFuse;

    float _timer = 0f;

    public override void InitProjectile(float damage, float knockbackForce, Vector3 forward, float velocity, bool gravity, bool pointCollision)
    {
        base.InitProjectile(damage, knockbackForce, forward, velocity, gravity, pointCollision);
    }

    public void InitProjectile(float damage, float knockbackForce, Vector3 forward, float velocity, bool gravity, bool pointCollision, float explosionDamage, float explosionForce, float explosionRadius, bool explodeOnContact, float explosionFuse)
    {
        InitProjectile(damage, knockbackForce, forward, velocity, gravity, pointCollision);
        _explosionDamage = explosionDamage;
        _explosionForce = explosionForce;
        _explosionRadius = explosionRadius;
        _explodeOnContact = explodeOnContact;
        _explosionFuse = explosionFuse;
    }

    protected override void FixedUpdate()
    {
        if (!_explodeOnContact)
            return;

        base.FixedUpdate();
    }

    void Update()
    {
        if (_isFired)
        {
            _timer += Time.deltaTime;
            if (_timer >= _explosionFuse)
            {
                Explode();
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!_explodeOnContact)
            return;

        base.OnCollisionEnter(collision);
    }
    protected override void HitTarget(Transform target, Vector3 hitPoint)
    {
        Explode();
    }

    void Explode()
    {
        Collider[] colliders =  Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(_explosionDamage);

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 0f, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
