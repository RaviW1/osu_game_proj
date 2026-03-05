public class DamageCommand : ICommand
{
    public void Execute(Player player)
    {
        player.TakeDamage();
    }
}