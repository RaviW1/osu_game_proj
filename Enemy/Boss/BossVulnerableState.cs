using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO: finish implementation 
// Right now is just a copy of the attack recovery state
// Should play one invincible animation entering vulerability
// then stay vulnerable for a certain amount of time
public class BossVulnerableState : IBossState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 5;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    private readonly double runDuration = 4.0; // Run for 3 seconds
    private int frameWidth = 655;
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(6, 4388, frameWidth, 578);
        commandReceivedThisFrame = false;
        timer = 0;
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int gap = 6;
        int startX = 6;

        int newX = startX + (currentFrame * (frameWidth + gap));

        // Update the boss's source rectangle
        // Note: Ensure the height (373 vs 395) is consistent with your sprite sheet
        boss.sourceRectangle = new Rectangle(newX, 4388, frameWidth, 578);
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
