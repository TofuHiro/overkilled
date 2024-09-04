using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyHealth : ObjectHealth
{
    public override void Die()
    {
        base.Die();

        DieServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void DieServerRpc()
    {
        Destroy(gameObject);
    }
}
