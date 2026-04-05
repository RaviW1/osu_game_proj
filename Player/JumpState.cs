using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class JumpState : IPlayerState
{
    private const float InitialVelocity = -400f;
    private const float ExtraForce = -800f;
    private const float MaxHoldTime = 0.4f;
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 12;

    private float jumpHeldTime = 0f;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    public void OnEnter(Player player)
    {
        SoundManager.PlaySFX("Jump");
        player.Velocity.Y = InitialVelocity;
        player.OnGround = false;
        player.IsAirborne = true;
        player.SuppressLandingTransition = true;
        jumpHeldTime = 0f;
        currentFrame = 0;
        timeSinceLastFrame = 0f;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        AdvanceFrame(dt);

        // Gravity handled by Player.Update
        // Transition to falling once moving downward
        if (player.Velocity.Y >= 0f)
        {
            player.SuppressLandingTransition = false;
            player.ChangeState(new FallingState());
        }
    }

    public void JumpHeld(Player player, float deltaTime)
    {
        if (jumpHeldTime >= MaxHoldTime) return;

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

        player.Velocity.X = direction * 3f;
        player.Position.X += player.Velocity.X;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    public void Jump(Player player) { }
    public void Heal(Player player) { }

    public void StopWalking(Player player) { }
    public void Attack(Player player) => player.ChangeState(new AttackState(wasJumping: true));
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());

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
