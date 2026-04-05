using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class FallingState : IPlayerState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 12;
    private const float WalkSpeed = 3f;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;

    public void OnEnter(Player player)
    {
        player.IsAirborne = true;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // DEBUG MESSAGE:
        // System.Console.WriteLine($"FallingState: OnGround={player.OnGround} VelY={player.Velocity.Y} PosY={player.Position.Y}");
        AdvanceFrame(dt);

        if (player.OnGround && !player.SuppressLandingTransition)
        {
            player.IsAirborne = false;
            player.ChangeState(new IdleState());
        }
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
        int fallingFrame = (TotalFrames - 1) - (currentFrame % TotalFrames);
        player.sourceRectangle = new Rectangle(
            fallingFrame * frameWidth, 0,
            frameWidth, player.CurrentTexture.Height);
    }

    public void Attack(Player player) => player.ChangeState(new AttackState(wasJumping: true));
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());
    public void Jump(Player player) { }
    public void Heal(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void StopWalking(Player player) { }

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
