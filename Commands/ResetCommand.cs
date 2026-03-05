using Microsoft.Xna.Framework;
using osu_game_proj;

public class ResetCommand : ICommand
{
    private readonly Game1 game;

    public ResetCommand(Game1 game)
    {
        this.game = game;
    }

    public void Execute(Player player)
    {
        game.Reset();
    }
}