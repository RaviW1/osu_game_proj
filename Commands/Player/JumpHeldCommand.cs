using Microsoft.Xna.Framework;

public class JumpHeldCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.JumpHeld((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}