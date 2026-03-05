using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class FallingState : IPlayerState
{
    private const float Gravity = 1200f;
    private const float GroundY = 200f;   // replace with collision later
    private const float WalkSpeed = 3f;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 12;

    public void Reset(Player player)
    {
        player.IsAirborne = true;
        // Velocity intentionally carries over from JumpState
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        player.Velocity.Y += Gravity * dt;
        player.Position += player.Velocity * dt;

        AdvanceFrame(dt);

        if (player.Position.Y >= GroundY)
        {
            player.Position.Y = GroundY;
            player.Velocity.Y = 0f;
            player.IsAirborne = false;
            player.ChangeState(new IdleState());
        }
    }

    public void Walk(Player player, int direction)
    {
        if (direction > 0) player.facing = SpriteEffects.None;
        else if (direction < 0) player.facing = SpriteEffects.FlipHorizontally;
        player.Position.X += direction * WalkSpeed;
    }

    public void Attack(Player player) => player.ChangeState(new AttackState(wasJumping: true));
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());
    public void Jump(Player player) { } // no double jump
    public void Heal(Player player) { }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        // Play frames in reverse to visually distinguish falling from rising
        int fallingFrame = (TotalFrames - 1) - (currentFrame % TotalFrames);
        player.sourceRectangle = new Rectangle(fallingFrame * frameWidth, 0, frameWidth, player.CurrentTexture.Height);
    }

    private void AdvanceFrame(float dt)
    {
        timeSinceLastFrame += dt;
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }
}