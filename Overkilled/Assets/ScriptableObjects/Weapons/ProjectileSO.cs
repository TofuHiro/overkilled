using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Weapons/Projectiles/New Projectile")]
public class ProjectileSO : ScriptableObject
{
    public GameObject prefab;
    public float projectileVelocity;
    public bool pointCollision;
    public bool projectileGravity;
}
