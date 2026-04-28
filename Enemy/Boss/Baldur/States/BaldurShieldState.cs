using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaldurShieldState : IBaldurState
{
    private const float SecondsPerFrame = 0.2f;
    private const int TotalFrames = 5;
    private float attackRange = 200f;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private double timer = 0;
    private Random rng;

    public void OnEnter(BaldurBoss boss)
    {
        boss.velocity = new Vector2(0, 0);
        boss.sourceRectangle = new Rectangle(2, 560, 296, 263);
        timer = 0;
        boss.Projectiles.Clear();
        rng = new Random();
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(BaldurBoss boss, GameTime gameTime)
    {
        bool animFinished = AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 294;
        int gap = 3;
        int startX = 4;
        int frameHeight = 260;

        int newX;
        int newY;
        if (currentFrame < 2)
        {
            newX = startX + (currentFrame * (frameWidth + gap));
            newY = 560;
        }
        else
        {
            newX = startX;
            newY = 1110;
            frameWidth = 254;
            frameHeight = 233;
        }

        boss.sourceRectangle = new Rectangle(newX, newY, frameWidth, frameHeight);
        timer += gameTime.ElapsedGameTime.TotalSeconds;

        // Logic for changing into new attack state
        if (Player.Instance != null)
        {
            Vector2 target = Player.Instance.Position;
            // Do attack logic
            Vector2 playerPos = Player.Instance.Position;
            float distance = Vector2.DistanceSquared(boss.position, playerPos);
            if (distance > (attackRange * attackRange))
            {
                boss.ChangeState(new BaldurAttackState());
            }
        }

    }
    public void Draw(BaldurBoss boss, SpriteBatch spriteBatch)
    {
    }
    private bool AdvanceFrame(float dt)
    {
        timeSinceLastFrame += dt;
        if (timeSinceLastFrame > SecondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            if (currentFrame == TotalFrames - 1)
            {
                return true;
            }
            currentFrame++;
        }
        return false;
    }
    public Rectangle GetBounds(BaldurBoss boss)
    {
        return Rectangle.Empty;
    }
}
