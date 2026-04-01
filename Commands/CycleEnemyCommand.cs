using Microsoft.Xna.Framework;
using osu_game_proj;

public class CycleEnemyCommand : ICommand
{
    private int direction;
    private GameScene scene;

    public CycleEnemyCommand(int direction, GameScene scene)
    {
        this.direction = direction;
        this.scene = scene;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        scene.CycleEnemy(direction);
    }
}