using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaldurIdleState : IBaldurState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 5;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private double timer = 0;
    private double idleDuration = 2.0; // start by idling for 2 seconds
    private Random rng;

    public void OnEnter(BaldurBoss boss)
    {
        boss.velocity = new Vector2(0, 0);
        boss.sourceRectangle = new Rectangle(3, 22, 284, 255);
        timer = 0;
        boss.Projectiles.Clear();
        rng = new Random();
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(BaldurBoss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 284;
        int gap = 3;
        int startX = 3;

        int newX = startX + (currentFrame * (frameWidth + gap));

        // Update the boss's source rectangle
        // Note: Ensure the height (373 vs 395) is consistent with sprite sheet
        boss.sourceRectangle = new Rectangle(newX, 22, frameWidth, 255);
        timer += gameTime.ElapsedGameTime.TotalSeconds;

        // Logic for changing into new attack state

        if (timer >= idleDuration)
        {
            // TODO: check if we should enter vulnerable state
            boss.ChangeState(new BaldurAttackState());
        }
    }
    public void Draw(BaldurBoss boss, SpriteBatch spriteBatch)
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
    public Rectangle GetBounds(BaldurBoss boss)
    {
        float scale = 0.5f;
        int scaledWidth = (int)(boss.sourceRectangle.Width * scale);
        int scaledHeight = (int)(boss.sourceRectangle.Height * scale);

        int bodyWidth = (int)(scaledWidth * 0.7f);
        int bodyHeight = (int)(scaledHeight * 0.9f);

        // Calculate X and Y based on the bottom-center origin
        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
