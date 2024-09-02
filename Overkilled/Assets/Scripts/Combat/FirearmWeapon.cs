using Unity.Netcode;
using UnityEngine;

public class FirearmWeapon : Weapon
{
    [Tooltip("The point where projectiles are fired from. Ensure the Z axis points forward")]
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

        DecreaseDurablity(1);
    }

    void SetAccuracy(float angle)
    {
        _aimConeAngle = angle;
    }

    void ShootProjectile()
    {
        ShootProjectileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ShootProjectileServerRpc()
    {
        float spread = Random.Range(-_aimConeAngle / 2f, _aimConeAngle / 2f);
        Quaternion rotation = Quaternion.Euler(_projectileExit.rotation.eulerAngles + (Vector3.up * spread));

        NetworkObject projectileNetworkObject = Instantiate(_firearmSO.projectile.prefab, _projectileExit.position, rotation).GetComponent<NetworkObject>();
        projectileNetworkObject.Spawn(true);

        Projectile projectile = projectileNetworkObject.GetComponent<Projectile>();
        projectile.InitProjectile(_firearmSO.damage, _firearmSO.knockbackForce, projectile.transform.forward);
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
