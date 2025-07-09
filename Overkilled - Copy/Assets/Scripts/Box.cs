using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Box : ObjectHealth, IDamagable
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
