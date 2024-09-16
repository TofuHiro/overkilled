public class EnemyHealth : ObjectHealth
{
    public override void Die()
    {
        base.Die();

        MultiplayerManager.Instance.DestroyObject(gameObject);
    }
}
