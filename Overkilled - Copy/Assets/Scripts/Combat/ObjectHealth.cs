using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectHealth : NetworkBehaviour, IDamagable
{
    [Tooltip("The type of entity this object is")]
    [SerializeField] EntityType _entityType;
    [Tooltip("The maximum possible health for this object")]
    [SerializeField] protected float _maxHealth;

    NetworkVariable <float> _currentHealth = new NetworkVariable<float>();
    NetworkObject _networkObject;

    protected virtual void Start()
    {
        _networkObject = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            _currentHealth.Value = _maxHealth;
    }

    public EntityType GetEntityType()
    {
        return _entityType;
    }

    public float GetHealth()
    {
        return _currentHealth.Value;
    }

    public void SetHealth(float value)
    {
        SetHealthServerRpc(value);
    }

    public virtual void TakeDamage(float damage)
    {
        SetHealthServerRpc(_currentHealth.Value - damage);
    }

    [Rpc(SendTo.Server)]
    void SetHealthServerRpc(float value)
    {
        _currentHealth.Value = value;

        if (_currentHealth.Value <= 0)
            Die();
    }

    public virtual void Die()
    {
        ObjectSpawner.Instance.DestroyObject(gameObject);
    }

    public NetworkObject GetNetworkObject()
    {
        return _networkObject;
    }
}
