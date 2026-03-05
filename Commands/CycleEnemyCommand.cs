using Microsoft.Xna.Framework;
using osu_game_proj; // ADD THIS LINE

public class CycleEnemyCommand : ICommand
{
    private int direction;
    
    public CycleEnemyCommand(int direction)
    {
        this.direction = direction;
    }
    
    public void Execute(Player player, GameTime gameTime)
    {
        Game1.CycleEnemy(direction);
    }
}
