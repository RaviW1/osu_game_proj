using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BossAttackAnticState : IBossState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 6;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private Vector2 offset = new Vector2(15, 10);
    private Random rng;

    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(5, 2945, 580, 400);
        if (boss.facingLeft)
        {

            offset = new Vector2(15, 10);
        }
        else
        {
            offset = new Vector2(-15, 10);
        }
        boss.OffsetPosition(offset);
        rng = new Random();
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        bool animFinished = AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here
        int frameWidth = 580;
        int gap = 5;
        int startX = 5;

        int newX = startX + (currentFrame * (frameWidth + gap));
        int newY;
        if (currentFrame < 5)
        {
            newX = startX + (currentFrame * (frameWidth + gap));
            newY = 2944;
        }
        else
        {
            newX = startX;
            newY = 3347;
        }
        boss.sourceRectangle = new Rectangle(newX, newY, frameWidth, 400);


        if (animFinished)
        {

            boss.OffsetPosition(-offset);
            float choice = rng.NextSingle();
            if (choice < .6)
            {
                boss.ChangeState(new BossAttackState());
            }
            else
            {
                boss.ChangeState(new BossJumpAttackState());
            }
        }

    }
    public void Draw(Boss boss, SpriteBatch spriteBatch)
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
    public Rectangle GetBounds(Boss boss)
    {
        float scale = 0.5f;
        int scaledWidth = (int)(boss.sourceRectangle.Width * scale);
        int scaledHeight = (int)(boss.sourceRectangle.Height * scale);

        int bodyWidth = (int)(scaledWidth * 0.3f);
        int bodyHeight = (int)(scaledHeight * 0.5f);

        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
