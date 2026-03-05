using Microsoft.Xna.Framework;

public class AttackCommand : ICommand
{
    public void Execute(Player player, GameTime gameTime)
    {
        player.Attack();
    }
}