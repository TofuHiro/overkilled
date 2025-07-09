using Unity.Netcode;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Tooltip("The point where projectiles are fired from. Ensure the Z axis points forward")]
    [SerializeField] Transform _projectileExit;
    [Tooltip("The projectile to use for this weapon")]
    [SerializeField] GameObject _projectilePrefab;

    ProjectileWeaponSO _projectileWeaponSO;
    float _aimConeAngle;

    void Awake()
    {
        _projectileWeaponSO = (ProjectileWeaponSO)_weaponSO;

        if (_projectileWeaponSO.GetType() != typeof(ProjectileWeaponSO))
            Debug.LogWarning("Warning. Incorrect WeaponSO type assigned to weapon " + name);
    }

    public override void Attack()
    {
        base.Attack();
        if (Durability <= 0)
            return;

        int n = 0;
        while (n < _projectileWeaponSO.numberOfProjectiles)
        {
            ShootProjectile();
            n++;
        }

        DecreaseDurablity(1);
    }

    void ShootProjectile()
    {
        ShootProjectileServerRpc();
    }

    [Rpc(SendTo.Server)]
    void ShootProjectileServerRpc()
    {
        float spread = Random.Range(-_aimConeAngle / 2f, _aimConeAngle / 2f);
        Quaternion rotation = Quaternion.Euler(_projectileExit.rotation.eulerAngles + (Vector3.up * spread));

        ObjectSpawner.Instance.SpawnObject(_projectilePrefab, _projectileExit.position, rotation);
    }

    public override void SetSecondaryAttackState(bool state)
    {
        base.SetSecondaryAttackState(state);
        if (state == true)
            SetAccuracy(_projectileWeaponSO.accurateAimConeAngle);
        else
            SetAccuracy(_projectileWeaponSO.normalAimConeAngle);
    }

    void SetAccuracy(float angle)
    {
        _aimConeAngle = angle;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_projectileExit.position, _projectileExit.forward);    
    }
}
