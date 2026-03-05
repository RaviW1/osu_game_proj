using osu_game_proj; // ADD THIS LINE

public class CycleEnemyCommand : ICommand
{
    private int direction;
    
    public CycleEnemyCommand(int direction)
    {
        this.direction = direction;
    }
    
    public void Execute(Player player)
    {
        Game1.CycleEnemy(direction);
    }
}
