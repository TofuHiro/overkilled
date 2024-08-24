using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Weapons/New Melee")]
public class MeleeSO : WeaponSO
{
    [Header("Melee")]
    public Vector3 areaOfAttack;

    [Header("Heavy Attack")]
    public float heavyKnockbackForce;
    public float heavyDamage;
    public Vector3 heavyAreaOfAttack;
    public float heavyAttackFrequency;
    public float stunTime;
    public bool flattenTarget;
}
