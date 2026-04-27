using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO: finish implementation
public class BossJumpAttackState : IBossState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 3;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    private readonly double runDuration = 4.0;
    private const float Gravity = 1200f;
    private const float JumpImpulse = -700f;
    private float horizontalSpeed = 200f;
    private float groundLevel;
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(3, 3771, 704, 593);
        commandReceivedThisFrame = false;
        timer = 0;

        groundLevel = boss.position.Y;

        // Calculate horizontal direction based on player or screen
        int dir = (Player.Instance.Position.X < boss.position.X) ? -1 : 1;
        boss.facingLeft = (dir == -1);

        boss.velocity = new Vector2(dir * horizontalSpeed, JumpImpulse);
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 704;
        int gap = 4;
        int startX = 3;

        int newX = startX + (currentFrame * (frameWidth + gap));

        // Update the boss's source rectangle
        // Note: Ensure the height (373 vs 395) is consistent with your sprite sheet
        boss.sourceRectangle = new Rectangle(newX, 3771, frameWidth, 593);
        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer >= runDuration)
        {
            boss.ChangeState(new BossIdleState());
        }

        Vector2 currentVel = boss.velocity;
        currentVel.Y += Gravity * dt;
        boss.velocity = currentVel;

        if (boss.position.Y >= groundLevel && boss.velocity.Y > 0)
        {
            // Snap to ground and stop moving
            boss.SetPos(new Vector2(boss.position.X, groundLevel));
            boss.velocity = Vector2.Zero;

            // Transition to next state
            boss.ChangeState(new BossIdleState());
        }
    }
    public void Draw(Boss boss, SpriteBatch spriteBatch)
    {
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
    public Rectangle GetBounds(Boss boss)
    {
        float scale = 0.5f;
        int scaledWidth = (int)(boss.sourceRectangle.Width * scale);
        int scaledHeight = (int)(boss.sourceRectangle.Height * scale);

        int bodyWidth = (int)(scaledWidth * 0.7f);
        int bodyHeight = (int)(scaledHeight * 0.5f);

        // Calculate X and Y based on the bottom-center origin
        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
