using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected ProjectileSO _projectileSO;

    Rigidbody _rb;
    float _damage;
    float _knockbackForce;
    bool _pointCollision;

    protected bool _isFired = false;
    Vector3 _prevPosition, _directionOfTravel;

    public virtual void InitProjectile(float damage, float knockbackForce, Vector3 forward)
    {
        _damage = damage;
        _knockbackForce = knockbackForce;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _projectileSO.projectileGravity;
        _rb.velocity = forward * _projectileSO.projectileVelocity;
        _pointCollision = _projectileSO.pointCollision;

        _isFired = true;
        _prevPosition = transform.position;
        _directionOfTravel = transform.forward;
    }

    protected virtual void FixedUpdate()
    {
        if (!_pointCollision)
            return;

        if (_isFired)
        {
            Physics.Linecast(_prevPosition, transform.position, out RaycastHit hit);
            if (hit.transform != null)
            {
                HitTarget(hit.transform, hit.point);
            }
        }

        _prevPosition = transform.position;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (_pointCollision)
            return;

        HitTarget(collision.transform, transform.position);
    }

    protected virtual void HitTarget(Transform target, Vector3 hitPoint)
    {
        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable != null) 
            damagable.TakeDamage(_damage);

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForceAtPosition(_directionOfTravel * _knockbackForce, hitPoint, ForceMode.Impulse);

        Destroy(gameObject);
    }
}
