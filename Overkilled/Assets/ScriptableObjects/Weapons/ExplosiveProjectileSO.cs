using UnityEngine;

[CreateAssetMenu(fileName = "New Explosive Projectiles", menuName = "Weapons/Projectiles/New Explosive Projectiles")]
public class ExplosiveProjectileSO : ProjectileSO
{
    [Header("Explosion")]
    [Tooltip("Damage inflicted by explosion")]
    public float explosionDamage;
    [Tooltip("The radius of the explosion")]
    public float explosionRadius;
    [Tooltip("Force applied to rigidbodies upon explosion")]
    public float explosionForce;
    [Tooltip("Whether to explode when colliding with an object. If false, the explosion is based of the fuse instead")]
    public bool explodeOnContact;
    [Tooltip("The maximum lifetime of this projectile before exploding manually")]
    public float explosionFuse;
}
