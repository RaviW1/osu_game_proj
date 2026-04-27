using Microsoft.Xna.Framework;

public class InterruptHealCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.InterruptHeal();
    }
}
