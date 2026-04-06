using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class Boofly : ISprite, IEnemy
{
    private Texture2D texture;
    private System.Numerics.Vector2 position;
    private System.Numerics.Vector2 velocity;
    private float bobTimer = 0f;
    private bool isDead = false;
    private float deathVelocityY = 0f;
    private const float floorY = 400f;

    public bool IsDead => isDead;


    public Boofly(Texture2D texture, System.Numerics.Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new System.Numerics.Vector2(50, 0);
    }

    public Microsoft.Xna.Framework.Rectangle GetBounds()
    {
        return new Microsoft.Xna.Framework.Rectangle(
            (int)position.X, (int)position.Y, 56, 64);
    }

    public void BounceX() { velocity.X *= -1; }
    public void BounceY() { velocity.Y *= -1; }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;

    public void TakeDamage()
    {
        isDead = true;
        velocity = System.Numerics.Vector2.Zero;
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

        if (position.X > 700 || position.X < 100)
        {
            velocity.X *= -1;
        }

        if (position.X > 700 || position.X < 100) velocity.X *= -1;
        bobTimer += 0.016f;
    }

    public void Draw(SpriteBatch spriteBatch, System.Numerics.Vector2 startCoords)
    {
        float bobOffset = (float)Math.Sin(bobTimer * 3) * 20;
        System.Numerics.Vector2 drawPos = new System.Numerics.Vector2(position.X, position.Y + bobOffset);

        var xnaDrawPos = new Microsoft.Xna.Framework.Vector2(drawPos.X, drawPos.Y);

        if (texture != null)
        {

            int frameWidth = 309;
            int frameHeight = 335;
            int frameX = 4;
            int frameY = 23;

            var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);

            // Scale it to reasonable size
            float scale = 0.2f;

            spriteBatch.Draw(texture, xnaDrawPos, sourceRect, Microsoft.Xna.Framework.Color.White,
                            0f, Microsoft.Xna.Framework.Vector2.Zero, scale,
                            Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}
