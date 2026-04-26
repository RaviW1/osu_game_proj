using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BossIdleState : IBossState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 5;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    private double idleDuration = 2.0; // start by idling for 2 seconds
    private readonly double runDuration = 4.0;
    private Random rng;

    public void OnEnter(Boss boss)
    {
        boss.velocity = new Vector2(0, 0);
        boss.sourceRectangle = new Rectangle(3, 25, 624, 390);
        commandReceivedThisFrame = false;
        timer = 0;
        rng = new Random();
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 621;
        int gap = 3;
        int startX = 3;

        int newX = startX + (currentFrame * (frameWidth + gap));

        // Update the boss's source rectangle
        // Note: Ensure the height (373 vs 395) is consistent with sprite sheet
        boss.sourceRectangle = new Rectangle(newX, 25, frameWidth, 390);
        timer += gameTime.ElapsedGameTime.TotalSeconds;

        // Logic for changing into new attack state

        if (timer >= idleDuration)
        {
            // TODO: check if we should enter vulnerable state


            // pick random state
            // Commented out for testing
            float choice = rng.NextSingle();
            if (choice < .4)
            {
                boss.ChangeState(new BossAttackAnticState());
            }
            else
            {
                boss.ChangeState(new BossRunState());
            }
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

        // Tighten the width to 30% of the sprite frame
        int bodyWidth = (int)(scaledWidth * 0.3f);
        // Usually, you want the hitbox slightly shorter than the head (e.g., 90% height)
        int bodyHeight = (int)(scaledHeight * 0.6f);

        // Calculate X and Y based on the bottom-center origin
        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
