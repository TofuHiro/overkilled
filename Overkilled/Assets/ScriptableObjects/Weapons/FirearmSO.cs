using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Firearm", menuName = "Weapons/New Firearm")]
public class FirearmSO : WeaponSO
{
    [Header("Firearm")]
    public ProjectileSO projectile;
    public int numberOfProjectiles;

    [Header("Accuracy")]
    public float normalAimConeAngle;
    public float accurateAimConeAngle;
}
