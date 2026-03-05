using osu_game_proj; // ADD THIS LINE
using Microsoft.Xna.Framework;
public class CycleBlockCommand : ICommand
{
    private int direction;

    public CycleBlockCommand(int direction)
    {
        this.direction = direction;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        Game1.CycleBlock(direction);
    }
}