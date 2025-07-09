using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeapon : Weapon
{
    RayWeaponSO _rayWeaponSO;
    float _aimConeAngle;

    Vector3 _gizmosEndpoint;

    void Awake()
    {
        _rayWeaponSO = (RayWeaponSO)_weaponSO;

        if (_rayWeaponSO.GetType() != typeof(RayWeaponSO))
            Debug.LogWarning("Warning. Incorrect WeaponSO type assigned to weapon " + name);
    }

    public override void Attack()
    {
        base.Attack();
        if (Durability <= 0)
            return;

        ShootRay();
        DecreaseDurablity(1);
    }

    void ShootRay()
    {
        float spread = Random.Range(-_aimConeAngle / 2f, _aimConeAngle / 2f);
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Vector3.up * spread));

        Physics.Raycast(_attackPosition.position, transform.forward, out RaycastHit hit, _rayWeaponSO.range, ~0, QueryTriggerInteraction.Ignore);

        //Gizmos
        _gizmosEndpoint = hit.collider ? hit.point : _attackPosition.position;

        if (hit.collider)
        {
            //Damage
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
                CombatManager.Instance.DamageTarget(damagable, _rayWeaponSO.damage, _rayWeaponSO.entityTarget);

            //Impact Force
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
                CombatManager.Instance.AddHitForce(rb, transform.forward * _rayWeaponSO.knockbackForce, hit.point);
        }
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);
        if (state == true)
            SetAccuracy(_rayWeaponSO.accurateAimConeAngle);
        else
            SetAccuracy(_rayWeaponSO.normalAimConeAngle);
    }

    void SetAccuracy(float angle)
    {
        _aimConeAngle = angle;
    }

    void OnDrawGizmos()
    {
        if (_attackPosition)
            Gizmos.DrawLine(_attackPosition.position, _gizmosEndpoint); 
    }
}
