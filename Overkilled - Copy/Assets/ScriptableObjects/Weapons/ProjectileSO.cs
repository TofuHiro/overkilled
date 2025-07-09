using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Weapons/Projectiles/New Projectile")]
public class ProjectileSO : ScriptableObject
{
    [Tooltip("The damage this projectile inflicts")]
    public float damage;
    [Tooltip("The knockback force of this projectile")]
    public float knockbackForce;
    [Tooltip("The entity types this weapon can damage")]
    public EntityType entityTarget;
    [Tooltip("The initial velocity of the projectile when fired")]
    public float projectileVelocity;
    [Tooltip("An optimised version of collision detection recommended for fast and small projectiles. Uses interval raycasting to check for collision. If left off, standard rigidbody detection is used instead (recommended for slow and large projectiles)")]
    public bool pointCollision;
    [Tooltip("Whether this projectile is affected by gravity")]
    public bool projectileGravity;

    [Header("Stun")]
    [Tooltip("The stun time applied to targets")]
    public float stunTime;
}
