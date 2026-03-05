using Microsoft.Xna.Framework;

public interface ICommand
{
    void Execute(Player player, GameTime gameTime);

}
