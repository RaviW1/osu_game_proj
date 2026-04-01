using osu_game_proj;
using Microsoft.Xna.Framework;

public class CycleStageCommand : ICommand
{
    private int direction;
    private GameScene scene;

    public CycleStageCommand(int direction, GameScene scene)
    {
        this.direction = direction;
        this.scene = scene;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        scene.CycleStage(direction);
    }
}