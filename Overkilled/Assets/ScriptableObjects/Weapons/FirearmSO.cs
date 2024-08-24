using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Firearm", menuName = "Weapons/New Firearm")]
public class FirearmSO : WeaponSO
{
    [Header("Firearm")]
    public GameObject projectile;
    public int numberOfProjectiles;
    public float projectileVelocity;
    public bool projectileGravity;
    public bool pointCollision;

    [Header("Accuracy")]
    public float normalAimConeAngle;
    public float accurateAimConeAngle;

    [Header("AOE Damage")]
    public bool useExplosion;
    public float explosionDamage;
    public float explosionRadius;
    public float explosionForce;
    public bool explodeOnContact;
    public float explosionFuse;
}
