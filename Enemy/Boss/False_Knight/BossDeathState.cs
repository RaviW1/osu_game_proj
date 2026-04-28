using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BossDeathState : IBossState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 8;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private double timer = 0;
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(1691, 11647, 419, 468);
        timer = 0;
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(Boss boss, GameTime gameTime)
    {
        AdvanceFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        // Update the source rectangle here

        if (currentFrame == 8)
        {
            boss.Die();
        }

        timer += gameTime.ElapsedGameTime.TotalSeconds;
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

        int bodyWidth = (int)(scaledWidth * 0.3f);
        int bodyHeight = (int)(scaledHeight * 0.5f);

        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
