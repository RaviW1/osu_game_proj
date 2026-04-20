using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WalkingState : IPlayerState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 9;
    private const float WalkSpeed = 5f;

    private int direction = 1;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;

    public WalkingState(int direction)
    {
        this.direction = direction;
    }

    public void OnEnter(Player player)
    {
        player.SuppressLandingTransition = false;
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(
            0, 0,
            player.CurrentTexture.Width / TotalFrames,
            player.CurrentTexture.Height);
        commandReceivedThisFrame = false;
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (!player.OnGround && player.Velocity.Y > 0)
        {
            player.IsAirborne = true;
            player.ChangeState(new FallingState());
            return;
        }

        // Set horizontal velocity instead of directly modifying position
        player.Velocity.X = direction * WalkSpeed;
        player.Position.X += player.Velocity.X;

        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void Walk(Player player, int direction)
    {
        if (direction == 0) return;

        commandReceivedThisFrame = true;
        this.direction = direction;

        if (direction > 0) //right
        {
            player.facing = SpriteEffects.None;

        }
        else if (direction < 0)//left
        {
            player.facing = SpriteEffects.FlipHorizontally;

        }


    }

    public void Draw(Player player, SpriteBatch spriteBatch)
    {
        player.CurrentTexture = player.Textures["Walking"];
        int frameWidth = player.CurrentTexture.Width / TotalFrames;
        player.sourceRectangle = new Rectangle(
            currentFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    public void StopWalking(Player player) => player.ChangeState(new IdleState());

    public void Jump(Player player) => player.ChangeState(new JumpState());
    public void Attack(Player player) => player.ChangeState(new AttackState());
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());
    public void Heal(Player player) { }
    // TODO: change so we switch to looking up walk cycle
    public void LookUp(Player player) { }

    private void AdvanceFrame(float dt)
    {
        timeSinceLastFrame += dt;
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }

    public void JumpHeld(Player player, float deltaTime) { }

    public void Dash(Player player)
    {
        if (player.TryDash())
            player.ChangeState(new DashState());
    }
}
