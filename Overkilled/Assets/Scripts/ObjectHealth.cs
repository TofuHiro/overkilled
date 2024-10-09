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

    float _currentHealth;
    NetworkObject _networkObject;

    protected virtual void Start()
    {
        _networkObject = GetComponent<NetworkObject>();

        _currentHealth = _maxHealth;
    }

    public EntityType GetEntityType()
    {
        return _entityType;
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public void SetHealth(float value)
    {
        SetHealthServerRpc(value);
    }

    public virtual void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);

        if (_currentHealth <= 0)
            Die();

        SetHealthServerRpc(_currentHealth);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetHealthServerRpc(float value)
    {
        SetHealthClientRpc(value);
    }
    [ClientRpc]
    void SetHealthClientRpc(float value)
    {
        _currentHealth = value;
    }

    public virtual void Die()
    {
        MultiplayerManager.Instance.DestroyObject(gameObject);
    }

    public NetworkObject GetNetworkObject()
    {
        return _networkObject;
    }
}
