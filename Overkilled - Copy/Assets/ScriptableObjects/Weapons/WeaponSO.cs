using UnityEngine;

public class WeaponSO : ScriptableObject
{
    [Tooltip("Base damage")]
    public float damage;
    [Tooltip("The frequency (seconds) this weapon can attack")]
    public float attackFrequency;
    [Tooltip("Whether this weapon is affected by durability or not")]
    public bool useDurability;
    [Tooltip("The maximum amount of times this weapon can attack or be used to deal damage")]
    public int durability;
    [Tooltip("If true, this weapon will only fire once per click/press")]
    public bool semiAutomatic;
    [Tooltip("The multiplier to add to the player's movement speed when the secondary attack mode is active (negative value for slower movement)")]
    public float secondaryAttackMovementSpeedMultiplier;

    [Header("Animations")]
    [Tooltip("Animation type to player when holding the weapon")]
    public IdleType idleType;
    [Tooltip("Animation type to play when attacking")]
    public AttackType attackType;
    [Tooltip("Animation type to play when holding secondary attack button")]
    public SecondaryType secondaryType;
    [Tooltip("Animation type to play when secondary attacking")]
    public AttackType secondaryAttackType;
}
