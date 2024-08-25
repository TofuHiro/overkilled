using UnityEngine;

[CreateAssetMenu(fileName = "New Firearm", menuName = "Weapons/New Firearm")]
public class FirearmSO : WeaponSO
{
    [Header("Firearm")]
    [Tooltip("Projectile ScriptableObject to fire for this weapon")]
    public ProjectileSO projectile;
    [Tooltip("Number of projectiles to fire at once per attack")]
    public int numberOfProjectiles;

    [Header("Accuracy")]
    [Tooltip("The angle of the aim cone when hipfiring")]
    public float normalAimConeAngle;
    [Tooltip("The angle of the aim cone when aiming")]
    public float accurateAimConeAngle;
}
