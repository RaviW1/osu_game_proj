using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class Boofly : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private float bobTimer = 0f;
    private bool isDead = false;
    private float deathVelocityY = 0f;
    private const float floorY = 6000f;

    private float patrolLeft;
    private float patrolRight;

    public bool IsDead => isDead;

    public Boofly(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(50, 0);

        this.patrolLeft = startPosition.X - 200f;
        this.patrolRight = startPosition.X + 200f;
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 56, 64);
    }

    public void BounceX() { velocity.X *= -1; }
    public void BounceY() { velocity.Y *= -1; }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;

    public void TakeDamage()
    {
        isDead = true;
        velocity = Vector2.Zero;
    }

    public void ResolveCollisions(List<CollisionResult> results)
    {
        foreach (var result in results)
        {
            if (result.IsHarmful)
            {
                TakeDamage();
                continue;
            }
            if (!result.IsCollideable) continue;
            switch (result.Direction)
            {
                case CollisionDirection.Left:
                case CollisionDirection.Right:
                    BounceX();
                    break;
                case CollisionDirection.Up:
                case CollisionDirection.Down:
                    BounceY();
                    break;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (isDead)
        {
            deathVelocityY += 20f;
            position.Y += deathVelocityY * 0.016f;
            if (position.Y >= floorY) position.Y = floorY;
            return;
        }

        position.X += velocity.X * 0.016f;

        if (position.X > patrolRight || position.X < patrolLeft)
        {
            velocity.X *= -1;
        }

        bobTimer += 0.016f;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        float bobOffset = (float)Math.Sin(bobTimer * 3) * 20;
        Vector2 drawPos = new Vector2(position.X, position.Y + bobOffset);

        if (texture != null)
        {
            int frameWidth = 309;
            int frameHeight = 335;
            int frameX = 4;
            int frameY = 23;
            var sourceRect = new Rectangle(frameX, frameY, frameWidth, frameHeight);
            float scale = 0.2f;
            spriteBatch.Draw(texture, drawPos, sourceRect, Color.White,
                            0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}