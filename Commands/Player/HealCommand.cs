using Microsoft.Xna.Framework;

public class HealCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.Heal();
    }
}