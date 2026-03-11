using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class FallingState : IPlayerState
{
    // Physics constants
    private const float Gravity = 1200f;
    //lower movement speed in air
    private const float WalkSpeed = 2.3f;

    // Animation constants
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 12;

    // Runtime state
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    // -------------------------------------------------------------------------
    // IPlayerState implementation
    // -------------------------------------------------------------------------

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

        //        foreach (Rectangle tile in Game1.GetCurrentLevelColliders())
        //        {
        //            if (player.GetBounds().Intersects(tile) && PhysicsHelper.IsLandingOnTile(player, tile))
        //            {
        //                PhysicsHelper.LandOnTile(player, tile);
        //                player.ChangeState(new IdleState());
        //                return;
        //            }
        //        }
    }

    //Air strafe
    public void Walk(Player player, int direction)
    {
        if (direction > 0) player.facing = SpriteEffects.None;
        else if (direction < 0) player.facing = SpriteEffects.FlipHorizontally;
        player.Position.X += direction * WalkSpeed;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];

        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        int fallingFrame = (TotalFrames - 1) - (currentFrame % TotalFrames);
        player.sourceRectangle = new Rectangle(
            fallingFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    // Transitions
    public void Attack(Player player) => player.ChangeState(new AttackState(wasJumping: true));
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());

    // No-ops
    public void Jump(Player player) { }
    public void Heal(Player player) { }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

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
