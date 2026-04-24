using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO: finish implementation
public class BossAttackAnticState : IBossState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 6;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    private readonly double runDuration = 4.0; // Run for 3 seconds
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(5, 2945, 580, 400);
        commandReceivedThisFrame = false;
        timer = 0;
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 580;
        int gap = 5;
        int startX = 5;

        int newX = startX + (currentFrame * (frameWidth + gap));
        int newY;
        if (currentFrame < 5)
        {
            // Frames 0-4 are on the original row
            newX = startX + (currentFrame * (frameWidth + gap));
            newY = 2944;
        }
        else
        {
            // Frame 5 (the 6th frame) is on the new row
            // Based on your screenshot, it looks like it starts at X=2 or 3
            newX = startX;
            newY = 3347;
        }
        // NOTE: Ensure the height (373 vs 395) is consistent with your sprite sheet
        boss.sourceRectangle = new Rectangle(newX, newY, frameWidth, 400);
        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer >= runDuration)
        {
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
    public void Run(Boss boss, int direction)
    {

    }
}
