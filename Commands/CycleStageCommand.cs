using osu_game_proj;
using Microsoft.Xna.Framework;
public class CycleStageCommand : ICommand
{
    private int direction;

    public CycleStageCommand(int direction)
    {
        this.direction = direction;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        Game1.CycleStage(direction);
    }
}


