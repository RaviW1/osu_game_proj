using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class AttackState : IPlayerState
{
    private const float AttackDuration = 0.3f;
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 6;

    private float attackTimer = 0f;
    private bool wasJumping = false;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    public AttackState(bool wasJumping = false)
    {
        this.wasJumping = wasJumping;
    }

    public void OnEnter(Player player)
    {
        player.CurrentTexture = player.Textures["Attack"];
        player.sourceRectangle = new Rectangle(896, 0, 128, 128);
        player.IsAttacking = true;

        // Always clear suppression on enter — attack should never inherit
        // stale landing suppression from a previous state
        if (!wasJumping)
            player.SuppressLandingTransition = false;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        attackTimer += dt;
        timeSinceLastFrame += dt;

        AdvanceFrame();

        if (attackTimer >= AttackDuration)
        {
            player.IsAttacking = false;

            if (wasJumping)
            {
                // Always clear suppression before returning to falling
                // so the player can land normally after an air attack
                player.SuppressLandingTransition = false;
                player.ChangeState(new FallingState());
            }
            else
            {
                player.ChangeState(new IdleState());
            }
        }
    }

    public void Walk(Player player, int direction)
    {
        if (!wasJumping) return;

        if (direction > 0)
            player.facing = SpriteEffects.None;
        else if (direction < 0)
            player.facing = SpriteEffects.FlipHorizontally;

        player.Velocity.X = direction * 3f;
        player.Position.X += player.Velocity.X;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Attacking"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    public void TakeDamage(Player player)
    {
        player.IsAttacking = false;
        player.SuppressLandingTransition = false;  // clean up before leaving
        player.ChangeState(new DamagedState());
    }

    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Heal(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void StopWalking(Player player) { }

    private void AdvanceFrame()
    {
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }
}