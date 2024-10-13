using UnityEngine;

public class WeaponSO : ScriptableObject
{
    [Tooltip("Base damage")]
    public float damage;
    [Tooltip("Force applied to rigidbodies hit by this weapon")]
    public float knockbackForce;
    [Tooltip("The frequency (seconds) this weapon can attack")]
    public float attackFrequency;
    [Tooltip("The maximum amount of times this weapon can attack or be used to deal damage")]
    public int durability;
    [Tooltip("If true, this weapon will only fire once per click/press")]
    public bool semiAutomatic;
    [Tooltip("The multiplier to apply to the player's movement speed when the secondary attack mode is active")]
    public float secondaryAttackMovementSpeedMultiplier;
}
