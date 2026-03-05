using Microsoft.Xna.Framework;
public class ShootFireballCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.ShootFireball();
    }
}