using Microsoft.Xna.Framework;

public class ResetCommand : ICommand
{
    private readonly GameScene scene;

    public ResetCommand(GameScene scene)
    {
        this.scene = scene;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        scene.Reset();
    }
}