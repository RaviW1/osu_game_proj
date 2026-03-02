using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class JumpState : IPlayerState
{
    private int currentFrame = 0;
    private int totalFrames = 12;
    private float timeSinceLastFrame = 0f;
    private float secondsPerFrame = .1f;

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 11 - 30, player.CurrentTexture.Height);
        player.Velocity.Y = -500f;
    }
    public void Update(Player player, GameTime gameTime)
    {
        // jump action
        player.Velocity.Y += 20f;
        player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;


        if (timeSinceLastFrame > secondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame++;
            if (currentFrame > totalFrames)
            {
                currentFrame = 0;
            }
        }

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
    public void Heal(Player player)
    {
        // Can't heal while moving/jumping/attacking/damaged - do nothing
    }
    public void Jump(Player player)
    {
        // do nothing, can't jump while jumping
    }
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState(wasJumping: true));
    }

    public void TakeDamage(Player player)
    {
        player.ChangeState(new DamagedState());
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];
        int frameIndex = currentFrame % 12;
        int frameWidth = player.CurrentTexture.Width / 12;
        int frameHeight = player.CurrentTexture.Height;

        int xPosition = frameIndex * frameWidth;
        player.sourceRectangle = new Rectangle(xPosition, 0, frameWidth, frameHeight);
    }
}