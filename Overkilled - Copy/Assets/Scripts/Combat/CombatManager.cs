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
            Debug.LogWarning("Warning. Multiple instances of CombatManager found. Destroying " + name);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Apply a damage value to a given target
    /// </summary>
    /// <param name="damagable">The damagable target</param>
    /// <param name="damage">The damage value</param>
    /// <param name="targetType">The entity type to damage</param>
    public void DamageTarget(IDamagable damagable, float damage, EntityType targetType)
    {
        if (damagable.GetEntityType() == targetType || damagable.GetEntityType() == EntityType.All || targetType == EntityType.All)
        {
            NetworkObject networkObject = damagable.GetNetworkObject();

            if (networkObject != null)
            {
                DamageTargetServerRpc(networkObject, damage);
            }
            else
            {
                Debug.LogError("Network Object could not be found for object " + name);
            }
        }
    }

    [Rpc(SendTo.Server)]
    void DamageTargetServerRpc(NetworkObjectReference targetNetworkObjectReference, float damage)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        IDamagable damagable = targetNetworkObject.GetComponent<IDamagable>();

        damagable.TakeDamage(damage);
    }

    /// <summary>
    /// Add a force to an object with a rigidbody
    /// </summary>
    /// <param name="rigidbody">The rigidbody of the object</param>
    /// <param name="force">The force to apply</param>
    /// <param name="hitPoint">The hit position to apply the force at</param>
    public void AddHitForce(Rigidbody rigidbody, Vector3 force, Vector3 hitPoint)
    {
        NetworkObject networkObject = rigidbody.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            AddHitForceServerRpc(networkObject, force, hitPoint);
        }
        else
        {
            Debug.LogError("Network Object could not be found for object " + name);
        }
    }

    [Rpc(SendTo.Server)]
    void AddHitForceServerRpc(NetworkObjectReference targetNetworkObjectReference, Vector3 force, Vector3 hitPoint)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        Rigidbody rigidbody = targetNetworkObject.GetComponent<Rigidbody>();

        rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    /// <summary>
    /// Add an explosive force to an object with a rigidbody
    /// </summary>
    /// <param name="rigidbody">The rigidbody of the object</param>
    /// <param name="explosiveForce">The explosive force to apply</param>
    /// <param name="explosionPosition">The position of the explosion</param>
    /// <param name="explosionRadius">The radius of the explosion</param>
    public void AddExplosiveForce(Rigidbody rigidbody, float explosiveForce, Vector3 explosionPosition, float explosionRadius)
    {
        NetworkObject networkObject = rigidbody.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            AddExplosiveForceServerRpc(networkObject, explosiveForce, explosionPosition, explosionRadius);
        }
        else
        {
            Debug.LogError("Network Object could not be found for object " + name);
        }
    }

    [Rpc(SendTo.Server)]
    void AddExplosiveForceServerRpc(NetworkObjectReference targetNetworkObjectReference, float explosiveForce, Vector3 explosionPosition, float explosionRadius)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        Rigidbody rigidbody = targetNetworkObject.GetComponent<Rigidbody>();

        rigidbody.AddExplosionForce(explosiveForce, explosionPosition, explosionRadius, 0f, ForceMode.Impulse);
    }

    public void StunTarget(IStunnable stunnable, float stunTime, bool flatten)
    {
        NetworkObject networkObject = stunnable.GetNetworkObject();
        if (networkObject != null)
        {
            StunTargetServerRpc(networkObject, stunTime, flatten);
        }
        else
        {
            Debug.LogError("Network Object could not be found for object " + name);
        }
    }

    [Rpc(SendTo.Server)]
    void StunTargetServerRpc(NetworkObjectReference targetNetworkObjectReference, float stunTime, bool flatten)
    {
        targetNetworkObjectReference.TryGet(out NetworkObject targetNetworkObject);
        IStunnable stunnable = targetNetworkObject.GetComponent<IStunnable>();

        stunnable.Stun(stunTime, flatten);
    }
}
