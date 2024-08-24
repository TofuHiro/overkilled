using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmWeapon : Weapon
{
    [SerializeField] Transform _projectileExit;

    FirearmSO _firearmSO;
    float _aimConeAngle;

    void Awake()
    {
        _firearmSO = (FirearmSO)_weaponSO;

        if (_firearmSO.GetType() != typeof(FirearmSO))
            Debug.LogWarning("Warning. Incorrect WeaponSO type assigned to weapon " + name);
    }

    protected override void Attack()
    {
        base.Attack();
        if (Durability <= 0)
            return;

        int n = 0;
        while (n < _firearmSO.numberOfProjectiles)
        {
            ShootProjectile();
            n++;
        }

        Durability--;
    }

    void SetAccuracy(float angle)
    {
        _aimConeAngle = angle;
    }

    void ShootProjectile()
    {
        float spread = Random.Range(-_aimConeAngle / 2f, _aimConeAngle / 2f);
        Quaternion rotation = Quaternion.Euler(_projectileExit.rotation.eulerAngles + (Vector3.up * spread));

        if (_firearmSO.useExplosion)
        {
            ExplosiveProjectile projectile = Instantiate(_firearmSO.projectile, _projectileExit.position, rotation).GetComponent<ExplosiveProjectile>();
            projectile.InitProjectile(
                _firearmSO.damage, 
                _firearmSO.knockbackForce, 
                projectile.transform.forward, 
                _firearmSO.projectileVelocity, 
                _firearmSO.projectileGravity,
                _firearmSO.pointCollision,
                _firearmSO.explosionDamage,
                _firearmSO.explosionForce,
                _firearmSO.explosionRadius,
                _firearmSO.explodeOnContact,
                _firearmSO.explosionFuse);
        }
        else
        {
            Projectile projectile = Instantiate(_firearmSO.projectile, _projectileExit.position, rotation).GetComponent<Projectile>();
            projectile.InitProjectile(
                _firearmSO.damage, 
                _firearmSO.knockbackForce,
                projectile.transform.forward, 
                _firearmSO.projectileVelocity, 
                _firearmSO.projectileGravity,
                _firearmSO.pointCollision);
        }
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);
        if (state == true)
            SetAccuracy(_firearmSO.accurateAimConeAngle);
        else
            SetAccuracy(_firearmSO.normalAimConeAngle);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_projectileExit.position, _projectileExit.forward);    
    }
}
