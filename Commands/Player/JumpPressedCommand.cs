public sealed class JumpPressedCommand : ICommand
{
    // Placeholder for later:
    // private readonly Player _player;

    public JumpPressedCommand()
    {
        // Later: accept Player in constructor
        // public JumpPressedCommand(Player player) => _player = player;
    }

    public void Execute(Player player)
    {
        player.Jump();


    }
}
