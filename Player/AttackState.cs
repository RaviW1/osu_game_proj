using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class AttackState : IPlayerState
{
    // Timing constants
    private const float AttackDuration = 0.3f;
    private const float Gravity = 1200f;
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 6;

    // Runtime state
    private float attackTimer = 0f;
    private bool wasJumping = false;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    public AttackState(bool wasJumping = false)
    {
        this.wasJumping = wasJumping;
    }

    // -------------------------------------------------------------------------
    // IPlayerState implementation
    // -------------------------------------------------------------------------

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Attack"];
        player.sourceRectangle = new Rectangle(896, 0, 128, 128);
        player.IsAttacking = true;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        attackTimer += dt;
        timeSinceLastFrame += dt;

        AdvanceFrame();

        if (wasJumping)
        {
            ApplyAirbornePhysics(player, dt);

            // Check if we landed on a tile mid-attack
            //            foreach (Rectangle tile in Game1.GetCurrentLevelColliders())
            //            {
            //                if (player.GetBounds().Intersects(tile) && PhysicsHelper.IsLandingOnTile(player, tile))
            //                {
            //                    PhysicsHelper.LandOnTile(player, tile);
            //                    player.IsAttacking = false;
            //                    player.ChangeState(new IdleState());
            //                    return;
            //                }
            //            }
        }

        // Attack finished — return to appropriate state
        if (attackTimer >= AttackDuration)
        {
            player.IsAttacking = false;
            player.ChangeState(wasJumping ? (IPlayerState)new FallingState() : new IdleState());
        }
    }

    public void Walk(Player player, int direction)
    {
        if (!wasJumping) return;

        if (direction > 0) player.facing = SpriteEffects.None;
        else if (direction < 0) player.facing = SpriteEffects.FlipHorizontally;
        player.Position.X += direction * 3f;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Attacking"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    // Transitions
    public void TakeDamage(Player player)
    {
        player.IsAttacking = false;
        player.ChangeState(new DamagedState());
    }

    // No-ops
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Heal(Player player) { }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void AdvanceFrame()
    {
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }

    private void ApplyAirbornePhysics(Player player, float dt)
    {
        player.Velocity.Y += Gravity * dt;
        player.Position += player.Velocity * dt;
    }
}
