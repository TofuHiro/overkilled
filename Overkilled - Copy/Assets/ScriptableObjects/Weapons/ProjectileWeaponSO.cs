using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Weapons/New Projectile Weapon")]
public class ProjectileWeaponSO : WeaponSO
{
    [Header("Projectile Weapon Settings")]
    [Tooltip("Number of projectiles to fire at once per attack")]
    public int numberOfProjectiles;

    [Header("Accuracy")]
    [Tooltip("The angle of the aim cone when hipfiring")]
    public float normalAimConeAngle;
    [Tooltip("The angle of the aim cone when aiming")]
    public float accurateAimConeAngle;
}
