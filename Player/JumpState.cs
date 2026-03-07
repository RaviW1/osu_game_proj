using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class JumpState : IPlayerState
{
    // Physics constants
    private const float InitialVelocity = -500f;
    private const float Gravity = 1200f;

    // Variable jump height — extra upward force applied while jump is held
    private const float ExtraForce = -800f;
    private const float MaxHoldTime = 0.4f;  // seconds of hold that benefit the jump

    // Animation constants
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 12;

    // Runtime state
    private float jumpHeldTime = 0f;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    // -------------------------------------------------------------------------
    // IPlayerState implementation
    // -------------------------------------------------------------------------

    public void Reset(Player player)
    {
        player.Velocity.Y = InitialVelocity;
        player.IsAirborne = true;
        jumpHeldTime = 0f;
        currentFrame = 0;
        timeSinceLastFrame = 0f;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        ApplyGravity(player, dt);
        AdvanceFrame(dt);

        // Hand off to FallingState once we start moving downward
        if (player.Velocity.Y >= 0f)
            player.ChangeState(new FallingState());
    }

    // Called every frame the jump button is held — boosts jump height up to MaxHoldTime
    public void JumpHeld(Player player, float deltaTime)
    {
        if (jumpHeldTime >= MaxHoldTime)
            return;

        float remaining = MaxHoldTime - jumpHeldTime;
        float applied = Math.Min(deltaTime, remaining);

        player.Velocity.Y += ExtraForce * applied;
        jumpHeldTime += deltaTime;
    }

    public void Walk(Player player, int direction)
    {
        if (direction > 0)
            player.facing = SpriteEffects.None;
        else if (direction < 0)
            player.facing = SpriteEffects.FlipHorizontally;

        player.Position.X += direction * 3f;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];

        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    // No-ops — behavior not permitted in this state
    public void Jump(Player player) { }
    public void Heal(Player player) { }

    // Transition to other states
    public void Attack(Player player) => player.ChangeState(new AttackState(wasJumping: true));
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void ApplyGravity(Player player, float dt)
    {
        player.Velocity.Y += Gravity * dt;
        player.Position += player.Velocity * dt;
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
