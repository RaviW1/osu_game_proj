public class ShootFireballCommand : ICommand
{
    public void Execute(Player player)
    {
        player.ShootFireball();
    }
}