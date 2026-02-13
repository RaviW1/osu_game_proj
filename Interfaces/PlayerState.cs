using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public interface IPlayerState
{
    void Update(Player player, GameTime gameTime);
    void Draw(Player player);
    void HandleInput(Player player, KeyboardController keyboardController);
}

public class IdleState : IPlayerState
{
    public void Update(Player player, GameTime gameTime)
    {
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

    }
    public void HandleInput(Player player, KeyboardController keyboardController)
    {

    }
}

public class WalkingState : IPlayerState
{
    private float offsetX = 0f;
    private int direction = 1;

    public void Update(Player player, GameTime gameTime)
    {
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

    }
    public void HandleInput(Player player, KeyboardController keyboardController)
    {

    }
}
