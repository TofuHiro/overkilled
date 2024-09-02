using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Box : NetworkBehaviour, IDamagable
{
    NetworkObject networkObject;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0f, 100f);
        }
    }
    private float _health;

    void Start()
    {
        _health = 100f;
        networkObject = GetComponent<NetworkObject>();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public NetworkObject GetNetworkObject()
    {
        return networkObject;
    }
}
