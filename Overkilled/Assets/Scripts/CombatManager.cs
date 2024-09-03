using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Warning. Multiple instances of Order System found. Destroying " + name);
            Destroy(Instance);
        }

        Instance = this;
    }

    public void DamageTarget(IDamagable damagable, float damage)
    {
        DamageTargetServerRpc(damagable.GetNetworkObject(), damage);
    }

    [ServerRpc(RequireOwnership = false)]
    void DamageTargetServerRpc(NetworkObjectReference targetNetworkObjectReference, float damage)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        IDamagable damagable = targetNetworkObject.GetComponent<IDamagable>();

        damagable.TakeDamage(damage);
    }

    public void AddHitForce(Rigidbody rigidbody, Vector3 force, Vector3 hitPoint)
    {
        NetworkObject networkObject = rigidbody.GetComponent<NetworkObject>();
        if (networkObject != null)
            AddHitForceServerRpc(networkObject, force, hitPoint);
        else
            Debug.LogError("Network Object could not be found for object " + name);
    }

    [ServerRpc(RequireOwnership = false)]
    void AddHitForceServerRpc(NetworkObjectReference targetNetworkObjectReference, Vector3 force, Vector3 hitPoint)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        Rigidbody rigidbody = targetNetworkObject.GetComponent<Rigidbody>();

        rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void AddExplosiveForce(Rigidbody rigidbody, float explosiveForce, Vector3 explosionPosition, float explosionRadius)
    {
        NetworkObject networkObject = rigidbody.GetComponent<NetworkObject>();
        AddExplosiveForceServerRpc(networkObject, explosiveForce, explosionPosition, explosionRadius);
    }

    [ServerRpc(RequireOwnership = false)]
    void AddExplosiveForceServerRpc(NetworkObjectReference targetNetworkObjectReference, float explosiveForce, Vector3 explosionPosition, float explosionRadius)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        Rigidbody rigidbody = targetNetworkObject.GetComponent<Rigidbody>();

        rigidbody.AddExplosionForce(explosiveForce, explosionPosition, explosionRadius, 0f, ForceMode.Impulse);
    }
}
