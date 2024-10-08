using Unity.Netcode;

public interface IDamagable
{
    public EntityType GetEntityType();
    public float GetHealth();
    public void SetHealth(float value);
    public void TakeDamage(float damage);
    public void Die();
    public NetworkObject GetNetworkObject();
}
