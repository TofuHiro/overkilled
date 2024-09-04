using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : ObjectHealth
{
    public override void Die()
    {
        base.Die();

        Debug.Log("Player Dead");
    }
}
