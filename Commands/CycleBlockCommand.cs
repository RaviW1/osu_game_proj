using Microsoft.Xna.Framework;
using osu_game_proj;

public class CycleBlockCommand : ICommand
{
    private int direction;
    private GameScene scene;

    public CycleBlockCommand(int direction, GameScene scene)
    {
        this.direction = direction;
        this.scene = scene;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        scene.CycleBlock(direction);
    }
}