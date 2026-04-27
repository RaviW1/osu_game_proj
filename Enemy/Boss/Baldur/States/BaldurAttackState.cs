using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaldurAttackState : IBaldurState
{
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 5;
    private float shootTimer = 0f;
    private float shootInterval = 1f;
    private float attackRange = 200f;

    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    private double timer = 0;
    private Random rng;
    public void OnEnter(BaldurBoss boss)
    {
        boss.velocity = new Vector2(0, 0);
        boss.sourceRectangle = new Rectangle(3, 22, 284, 255);
        timer = 0;
        rng = new Random();
    }
    // AI-Written (Wrote the math logic to get new source Rectangles)
    public void Update(BaldurBoss boss, GameTime gameTime)
    {
        timer += gameTime.ElapsedGameTime.TotalSeconds;

        // logic for shooting fireballs
        shootTimer += 0.016f;
        if (shootTimer >= shootInterval) { ShootFireball(boss); shootTimer = 0f; }

        for (int i = boss.Projectiles.Count - 1; i >= 0; i--)
        {
            boss.Projectiles[i].Update(gameTime);
            var projPos = boss.Projectiles[i].GetPosition();
            if (Math.Abs(projPos.X - boss.position.X) > 800 || Math.Abs(projPos.Y - boss.position.Y) > 600)
                boss.Projectiles.RemoveAt(i);
        }

        // Logic for changing into new state

        if (Player.Instance != null)
        {
            Vector2 target = Player.Instance.Position;
            // Do attack logic
            Vector2 playerPos = Player.Instance.Position;
            float distance = Vector2.DistanceSquared(boss.position, playerPos);
            if (distance <= (attackRange * attackRange))
            {
                boss.ChangeState(new BaldurShieldState());
            }
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

        int bodyWidth = (int)(scaledWidth * 0.3f);
        int bodyHeight = (int)(scaledHeight * 0.6f);

        // Calculate X and Y based on the bottom-center origin
        int x = (int)boss.position.X - (bodyWidth / 2);
        int y = (int)boss.position.Y - bodyHeight;

        return new Rectangle(x, y, bodyWidth, bodyHeight);
    }
    // copied from the aspid code
    private void ShootFireball(BaldurBoss boss)
    {
        // by design the baldur enemy always faces left
        Vector2 fireballVelocity = new Vector2(-150, 0);
        Vector2 pos = new Vector2(boss.position.X - 70, boss.position.Y - 70);
        boss.Projectiles.Add(new Projectile(boss.fireballTexture, pos, fireballVelocity));
    }
}
