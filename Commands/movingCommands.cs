using Microsoft.Xna.Framework;

public class WalkCommand : ICommand
{
    private int direction;
    public WalkCommand(int direction)
    {
        this.direction = direction;
    }
    public void Execute(Player player)
    {
        player.Walk(direction);
    }
}
