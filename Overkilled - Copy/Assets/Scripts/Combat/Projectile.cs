using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [Tooltip("Projectile Scriptable Object for this projectile")]
    [SerializeField] protected ProjectileSO _projectileSO;

    Rigidbody _rb;

    protected bool _isFired = false;
    Vector3 _prevPosition, _directionOfTravel;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _projectileSO.projectileGravity;
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(InitProjectile());
    }

    IEnumerator InitProjectile()
    {
        yield return null;

        _prevPosition = transform.position;
        _directionOfTravel = transform.forward;
        _rb.AddForce(_directionOfTravel * _projectileSO.projectileVelocity, ForceMode.Impulse);
        _isFired = true;
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
        HitTarget(collision.transform, transform.position);
    }

    protected virtual void HitTarget(Transform target, Vector3 hitPoint)
    {
        //Damage
        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable != null)
            CombatManager.Instance.DamageTarget(damagable, _projectileSO.damage, _projectileSO.entityTarget);

        //Apply Force
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
            CombatManager.Instance.AddHitForce(rb, _directionOfTravel * _projectileSO.knockbackForce, hitPoint);

        //Stun
        IStunnable stunnable = target.GetComponent<IStunnable>();
        if (stunnable != null)
            CombatManager.Instance.StunTarget(stunnable, _projectileSO.stunTime, false);

        ObjectSpawner.Instance.DestroyObject(gameObject);
    }
}
