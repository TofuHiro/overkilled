using UnityEngine;

[CreateAssetMenu(fileName = "New Ray Gun", menuName = "Weapons/New Ray Gun")]
public class RayWeaponSO: WeaponSO
{
    [Header("Ray Settings")]
    [Tooltip("The entity types this weapon can damage")]
    public EntityType entityTarget;
    [Tooltip("Maximum ray distance")]
    public float range;

    [Header("Accuracy")]
    [Tooltip("The angle of the aim cone when hipfiring")]
    public float normalAimConeAngle;
    [Tooltip("The angle of the aim cone when aiming")]
    public float accurateAimConeAngle;
}
