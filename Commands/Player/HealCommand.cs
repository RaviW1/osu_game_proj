public class HealCommand : ICommand
{
    public void Execute(Player player)
    {
        player.Heal();
    }
}