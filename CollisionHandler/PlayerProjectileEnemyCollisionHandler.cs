public class PlayerProjectileEnemyCollisionHandler
{
    public void HandleCollision(Aspid aspid)
    {
        aspid.TakeDamage();
    }

    public void HandleCollision(Boofly boofly)
    {
        boofly.TakeDamage();
    }

    public void HandleCollision(HuskBully huskBully)
    {
        huskBully.TakeDamage();
    }
}