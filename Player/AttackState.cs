using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class AttackState : IPlayerState
{
    private float attackTimer = 0f;
    private const float attackDuration = 0.3f;
    private bool wasJumping = false;
    private int currentFrame = 0;
    private int totalFrames = 6;
    private float timeSinceLastFrame = 0f;
    private float secondsPerFrame = .1f;

    private const float Gravity = 1200f;

    public AttackState(bool wasJumping = false)
    {
        this.wasJumping = wasJumping;
    }

    public void Update(Player player, GameTime gameTime)
    {
        attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

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

        // Replace the wasJumping block in Update with this:
        if (wasJumping)
        {
            player.Velocity.Y += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (player.Position.Y >= 370f)
            {
                player.Position.Y = 370f;
                player.Velocity.Y = 0f;
                player.IsAirborne = false;
                player.ChangeState(new IdleState());
                return;
            }
        }

        if (attackTimer >= attackDuration)
        {
            player.IsAttacking = false;
            player.ChangeState(wasJumping ? (IPlayerState)new FallingState() : new IdleState());
        }

        if (attackTimer >= attackDuration)
        {
            if (wasJumping)
            {
                // this line is causing a double jump
                // TODO: fix double jump
                //player.ChangeState(new JumpState());

                // this will change later when we implement collision detection etc
                if (player.Position.Y >= 370)
                {
                    player.Position.Y = 370;
                    player.IsAttacking = false;
                    player.ChangeState(new IdleState());
                }
            }
            else
                player.ChangeState(new IdleState());
        }
    }

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Attack"];
        int frameWidth = 128;
        int frameHeight = 128;
        int frameX = 896;
        int frameY = 0;
        player.sourceRectangle = new Rectangle(frameX, frameY, frameWidth, frameHeight);
        player.IsAttacking = true;
    }

    public void Walk(Player player, int direction)
    {
        if (wasJumping)
        {
            if (direction > 0)
                player.facing = SpriteEffects.None;
            else if (direction < 0)
                player.facing = SpriteEffects.FlipHorizontally;
            player.Position.X += direction * 3f;
        }
    }
    public void Heal(Player player)
    {
        // Can't heal while moving/jumping/attacking/damaged - do nothing
    }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Attacking"];
        int frameIndex = currentFrame % totalFrames;
        int frameWidth = player.CurrentTexture.Width / totalFrames;
        int frameHeight = player.CurrentTexture.Height;

        int xPosition = frameIndex * frameWidth;
        player.sourceRectangle = new Rectangle(xPosition, 0, frameWidth, frameHeight);
    }
    public void TakeDamage(Player player)
    {
        player.IsAttacking = false;
        player.ChangeState(new DamagedState());
    }
}
