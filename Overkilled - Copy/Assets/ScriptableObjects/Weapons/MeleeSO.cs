using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Weapons/New Melee")]
public class MeleeSO : WeaponSO
{
    [Tooltip("Force applied to rigidbodies hit by this weapon")]
    public float knockbackForce;

    [Header("Melee Settings")]
    [Tooltip("The entity types this weapon can damage")]
    public EntityType entityTarget;
    [Tooltip("The size of the 3D space to check for collisions")]
    public Vector3 areaOfAttack;

    [Header("Heavy Attack")]
    [Tooltip("Force applied to rigidbodies hit by heavy attack with this weapon")]
    public float heavyKnockbackForce;
    [Tooltip("Damage inflicted with heavy attack")]
    public float heavyDamage;
    [Tooltip("The size of the 3D space to check for collisions for heavy attacks")]
    public Vector3 heavyAreaOfAttack;
    [Tooltip("The frequency (seconds) this weapon can perform heavy attacks")]
    public float heavyAttackFrequency;
    [Tooltip("The stun time applied to targets")]
    public float stunTime;
    [Tooltip("The visual effect of flattening targets when hit with heavy attacks")]
    public bool flattenTarget;
}
