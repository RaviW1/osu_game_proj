using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WalkingState : IPlayerState
{
    private int direction = 1;
    private int currentFrame = 0;
    private int totalFrames = 8;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = true;
    private float secondsPerFrame = .1f;

    public WalkingState(int direction)
    {
        this.direction = direction;
        this.currentFrame = 0;
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (!commandReceivedThisFrame)
        {
            player.ChangeState(new IdleState());
            return;
        }

        commandReceivedThisFrame = false;

        timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;


        if (timeSinceLastFrame > secondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame++;
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
        }
    }
    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        int frameIndex = currentFrame % 9;
        int frameWidth = player.CurrentTexture.Width / 9;
        int frameHeight = player.CurrentTexture.Height;


        int xPosition = frameIndex * frameWidth;

        player.sourceRectangle = new Rectangle(xPosition, 0, frameWidth, frameHeight);
    }
    public void Walk(Player player, int direction)
    {
        commandReceivedThisFrame = true;
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
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState());
    }
    public void TakeDamage(Player player)
    {
        player.ChangeState(new DamagedState());
    }
    public void Heal(Player player)
    {
        // Can't heal while walking - do nothing
    }
}