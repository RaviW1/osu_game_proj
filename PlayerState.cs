using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public interface IPlayerState
{
    void Update(Player player, GameTime gameTime);
    void Draw(Player player);
    void Walk(Player player, int direction);
    void Jump(Player player);
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
    public void Walk(Player player, int direction)
    {
        player.ChangeState(new WalkingState(direction));
    }
    public void Jump(Player player)
    {
        player.ChangeState(new JumpState());
    }
}

public class WalkingState : IPlayerState
{
    private float offsetX = 0f;
    private int direction = 1;

    public WalkingState(int direction)
    {
        this.direction = direction;
    }

    public void Update(Player player, GameTime gameTime)
    {
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

    }
    public void Walk(Player player, int direction)
    {
        if (direction > 0)
        {
            player.facing = SpriteEffects.None;
        }
        else if (direction < 0)
        {
            player.facing = SpriteEffects.FlipHorizontally;
        }
        player.Position.X += direction * 5f;
    }
    public void Jump(Player player)
    {
        player.ChangeState(new JumpState());
    }
}

public class JumpState : IPlayerState
{
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
        player.Velocity.Y = -500f;
    }
    public void Update(Player player, GameTime gameTime)
    {
        // jump action
        player.Velocity.Y += 20f;
        player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // we decide 200 is the "floor"
        // this will change later when we implement collision detection etc
        if (player.Position.Y >= 200)
        {
            player.Position.Y = 200;
            player.ChangeState(new IdleState());
        }
    }
    public void Walk(Player player, int direction)
    {
        // allow slight changes in direction when jumping
        if (direction > 0)
        {
            player.facing = SpriteEffects.None;
        }
        else if (direction < 0)
        {
            player.facing = SpriteEffects.FlipHorizontally;
        }
        player.Position.X += direction * 3f;
    }
    public void Jump(Player player)
    {
        // do nothing, can't jump while jumping
    }
}
