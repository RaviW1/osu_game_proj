using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BossRunState : IBossState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 5;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    private readonly double runDuration = 4.0; // Run for 3 seconds
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(3, 1256, 623, 490);
        commandReceivedThisFrame = false;
        timer = 0;
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 623;
        int gap = 3;
        int startX = 3;

        int newX = startX + (currentFrame * (frameWidth + gap));

        // Update the boss's source rectangle
        // Note: Ensure the height (373 vs 395) is consistent with your sprite sheet
        boss.sourceRectangle = new Rectangle(newX, 1256, frameWidth, 490);
        timer += gameTime.ElapsedGameTime.TotalSeconds;
<<<<<<< Updated upstream
        if (timer >= runDuration)
        {
            boss.ChangeState(new BossIdleState());
        }
=======
>>>>>>> Stashed changes
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
    public void Run(Boss boss, int direction)
    {

    }
}
