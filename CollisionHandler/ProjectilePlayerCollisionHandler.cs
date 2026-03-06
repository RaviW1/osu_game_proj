using Microsoft.Xna.Framework;

public class ProjectilePlayerCollisionHandler
{
    public void HandleCollision(Player player, Projectile projectile)
    {
        player.PlayerHealth--;
        player.TakeDamage();
    }
}