using Microsoft.Xna.Framework;

public class DashCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.Dash();
    }
}