using Microsoft.Xna.Framework;

public class DamageCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.TakeDamage();
    }
}