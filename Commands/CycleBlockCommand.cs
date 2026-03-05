using osu_game_proj; // ADD THIS LINE

public class CycleBlockCommand : ICommand
{
    private int direction;

    public CycleBlockCommand(int direction)
    {
        this.direction = direction;
    }

    public void Execute(Player player)
    {
        Game1.CycleBlock(direction);
    }
}