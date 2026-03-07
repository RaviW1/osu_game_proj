using osu_game_proj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

    public List<ICommand> GetCommands(GameTime gameTime)
    {
        var activeCommands = new List<ICommand>();
        MouseState current = Mouse.GetState();

        // Detect Left Click
        if (current.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
        {
            int w = game.GraphicsDevice.Viewport.Width;
            int h = game.GraphicsDevice.Viewport.Height;

            bool isLeftHalf = current.X < w / 2;
            bool isTopHalf = current.Y < h / 2;


            if (isLeftHalf && isTopHalf)
            {
                if (topLeft != null) activeCommands.Add(topLeft);
            }
            else if (!isLeftHalf && isTopHalf)
            {
                if (topRight != null) activeCommands.Add(topRight);
            }
            // Add similar logic for bottom quadrants if needed
        }

        // Detect Right Click (Quit)
        if (current.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released)
        {
            if (quit != null) activeCommands.Add(quit);
        }

        previousState = current;
        return activeCommands;
    }
}

