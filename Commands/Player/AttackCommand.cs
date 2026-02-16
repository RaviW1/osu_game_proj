public class AttackCommand : ICommand
{
    public void Execute(Player player)
    {
        player.Attack();
    }
}