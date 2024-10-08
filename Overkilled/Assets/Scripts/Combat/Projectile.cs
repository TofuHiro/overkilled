using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [Tooltip("Projectile Scriptable Object for this projectile")]
    [SerializeField] protected ProjectileSO _projectileSO;

    Rigidbody _rb;
    float _damage;
    float _knockbackForce;

    protected bool _isFired = false;
    Vector3 _prevPosition, _directionOfTravel;

    /// <summary>
    /// Initialize this projectile with base stats
    /// </summary>
    /// <param name="damage">The collision damage of this projectile</param>
    /// <param name="knockbackForce">Force applied upon collision</param>
    /// <param name="forward">The direction this projectile to be fired towards</param>
    public virtual void InitProjectile(float damage, float knockbackForce, Vector3 forward)
    {
        InitProjectileServerRpc(damage, knockbackForce, forward);
    }

    [ServerRpc(RequireOwnership = false)]
    void InitProjectileServerRpc(float damage, float knockbackForce, Vector3 forward)
    {
        _damage = damage;
        _knockbackForce = knockbackForce;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _projectileSO.projectileGravity;

        _isFired = true;
        _prevPosition = transform.position;
        _directionOfTravel = transform.forward;
        _rb.AddForce(forward * _projectileSO.projectileVelocity, ForceMode.Impulse);
    }

    protected virtual void FixedUpdate()
    {
        if (!_projectileSO.pointCollision)
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
        if (_projectileSO.pointCollision)
            return;

        HitTarget(collision.transform, transform.position);
    }

    protected virtual void HitTarget(Transform target, Vector3 hitPoint)
    {
        //Damage
        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable != null)
            CombatManager.Instance.DamageTarget(damagable, _damage, _projectileSO.entityTarget);

        //Apply Force
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
            CombatManager.Instance.AddHitForce(rb, _directionOfTravel * _knockbackForce, hitPoint);

        //Stun
        IStunnable stunnable = target.GetComponent<IStunnable>();
        if (stunnable != null)
            CombatManager.Instance.StunTarget(stunnable, _projectileSO.stunTime, false);

        MultiplayerManager.Instance.DestroyObject(gameObject);
    }
}
