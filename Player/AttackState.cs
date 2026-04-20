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

    public void Draw(Player player, SpriteBatch spriteBatch)
    {
        player.CurrentTexture = player.Textures["Attacking"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);

        Vector2 slashPos = new Vector2((int)player.Position.X + 10, (int)player.Position.Y);

        Texture2D slashTexture = player.Textures["side_slash"];
        Rectangle slashSource = new Rectangle(0, 0, slashTexture.Width, slashTexture.Height);

        Rectangle slashSourceRectangle = new Rectangle(
            frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
        Vector2 slashOrigin = new Vector2(slashSource.Width / 2f, (slashSource.Height / 1.5f));
        spriteBatch.Draw(slashTexture, slashPos, slashSource, Color.White, 0f, slashOrigin, 0.2f, player.facing, 0f);
    }

    public void TakeDamage(Player player)
    {
        player.IsAttacking = false;
        player.SuppressLandingTransition = false;
        player.ChangeState(new DamagedState());
    }

    public void Dash(Player player)
    {
        if (player.TryDash())
        {
            player.IsAttacking = false;
            player.SuppressLandingTransition = false;
            player.ChangeState(new DashState());
        }
    }

    public void DrawSlashEffects()
    {

    }

    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Heal(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void StopWalking(Player player) { }
    // cant change attack direction will attacking?
    public void LookUp(Player player) { }

    private void AdvanceFrame()
    {
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }
}
