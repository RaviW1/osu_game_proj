using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BossRunState : IBossState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 5;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private double timer = 0;
    private readonly double runDuration = 4.0; // Run for 3 seconds
    private float runSpeed = 400f;
    public void OnEnter(Boss boss)
    {
        boss.sourceRectangle = new Rectangle(3, 1256, 623, 490);
        timer = 0;

        // first check which half of the screen we are on

        int direction;
        if (boss.position.X < 400)
        {
            boss.facingLeft = false;
            direction = 1;
        }
        else
        {
            boss.facingLeft = true;
            direction = -1;
        }
        boss.velocity = new Vector2(direction * runSpeed, 0);
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

        boss.sourceRectangle = new Rectangle(newX, 1256, frameWidth, 490);
        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer >= runDuration)
        {
            boss.ChangeState(new BossAttackAnticState());
        }


        // logic for changing direction once reaching end of run

        if (boss.position.X < 30)
        {
            boss.facingLeft = false;
            boss.ChangeState(new BossIdleState());
        }
        else if (boss.position.X > 750)
        {
            boss.facingLeft = true;
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

        int bodyWidth = (int)(scaledWidth * 0.3f);
        int bodyHeight = (int)(scaledHeight * 0.5f);

        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
}
