using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Explosive Projectiles", menuName = "Weapons/Projectiles/New Explosive Projectiles")]
public class ExplosiveProjectileSO : ProjectileSO
{
    [Header("Explosion")]
    public float explosionDamage;
    public float explosionRadius;
    public float explosionForce;
    public bool explodeOnContact;
    public float explosionFuse;
}
