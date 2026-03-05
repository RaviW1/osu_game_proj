using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WalkingState : IPlayerState
{
    // Animation constants
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 9;
    private const float WalkSpeed = 5f;

    // Runtime state
    private int direction = 1;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = true;

    public WalkingState(int direction)
    {
        this.direction = direction;
    }

    // -------------------------------------------------------------------------
    // IPlayerState implementation
    // -------------------------------------------------------------------------

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / TotalFrames, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (!commandReceivedThisFrame)
        {
            player.ChangeState(new IdleState());
            return;
        }

        commandReceivedThisFrame = false;

        // Movement applied once per frame regardless of how many keys are held
        player.Position.X += direction * WalkSpeed;

        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    // Stores intent only — actual movement applied in Update
    public void Walk(Player player, int direction)
    {
        commandReceivedThisFrame = true;

        if (direction > 0)
            player.facing = SpriteEffects.None;
        else if (direction < 0)
            player.facing = SpriteEffects.FlipHorizontally;

        this.direction = direction;
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    public void Jump(Player player) => player.ChangeState(new JumpState());
    public void Attack(Player player) => player.ChangeState(new AttackState());
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());

    // No-ops — behavior not permitted in this state
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