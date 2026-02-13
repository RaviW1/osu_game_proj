using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Windows.Input;

public class MouseController : IController
{
    private readonly Game1 game;
    private readonly ICommand topLeft;
    private readonly ICommand topRight;
    private readonly ICommand bottomLeft;
    private readonly ICommand bottomRight;
    private readonly ICommand quit;

    private MouseState previousState;

    public MouseController(
        Game1 game,
        ICommand topLeft,
        ICommand topRight,
        ICommand bottomLeft,
        ICommand bottomRight,
        ICommand quit)
    {
        this.game = game;
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        this.quit = quit;

        previousState = Mouse.GetState();
    }

    public void Update()
    {
        MouseState current = Mouse.GetState();

        if (current.RightButton == ButtonState.Pressed &&
            previousState.RightButton == ButtonState.Released)
        {
            quit.Execute();
        }

        if (current.LeftButton == ButtonState.Pressed &&
            previousState.LeftButton == ButtonState.Released)
        {
            int w = game.GraphicsDevice.Viewport.Width;
            int h = game.GraphicsDevice.Viewport.Height;

            bool left = current.X < w / 2;
            bool top = current.Y < h / 2;

            if (left && top) topLeft.Execute();
            else if (!left && top) topRight.Execute();
            else if (left && !top) bottomLeft.Execute();
            else bottomRight.Execute();
        }

        previousState = current;
    }
}
