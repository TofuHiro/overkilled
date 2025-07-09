using Unity.Netcode;

public interface IStunnable 
{
    public void Stun(float time, bool flatten);
    public NetworkObject GetNetworkObject();
}
