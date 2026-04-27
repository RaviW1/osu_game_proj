using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaldurDeathState : IBaldurState
{
    private const float SecondsPerFrame = 0.15f;
    private const int TotalFrames = 8;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = false;
    private double timer = 0;
    public void OnEnter(BaldurBoss boss)
    {
        boss.sourceRectangle = new Rectangle(451, 3074, 442, 315);
        commandReceivedThisFrame = false;
        boss.Projectiles.Clear();
        timer = 0;
    }
    public void Update(BaldurBoss boss, GameTime gameTime)
    {
        // Update the source rectangle here
        timer += gameTime.ElapsedGameTime.TotalSeconds;
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

        int bodyWidth = (int)(scaledWidth * 0.3f);
        int bodyHeight = (int)(scaledHeight * 0.5f);

        // Calculate X and Y based on the bottom-center origin
        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
