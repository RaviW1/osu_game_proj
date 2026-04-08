using Microsoft.Xna.Framework;

public class InteractCommand : ICommand
{
    private readonly GameScene _scene;

    public InteractCommand(GameScene scene)
    {
        _scene = scene;
    }

    public void Execute(Player player, GameTime gameTime)
    {
        _scene.TriggerInteract();
    }
}