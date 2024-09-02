using Unity.Netcode;

public interface IDamagable
{
    public float Health { get; set; }
    public void TakeDamage(float damage);
    public NetworkObject GetNetworkObject();
}
