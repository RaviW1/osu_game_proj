using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class HuskBully : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private bool facingLeft;
    private bool isDead;
    private float floorY = 400f;
    private int currentFrame;
    private Rectangle[] frames = new Rectangle[8];
    private TimeSpan delay;
    private TimeSpan elapsedTime;

    public bool IsDead => isDead;

    public HuskBully(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(-1, 0);
        this.facingLeft = true;
        this.isDead = false;
        this.currentFrame = 0;
        for (int i = 0; i < 7; i++)
            this.frames[i] = new Rectangle(3 + 111 * i, 174, 107, 128);
        this.frames[7] = new Rectangle(492, 1165, 159, 110);
        this.delay = TimeSpan.FromSeconds(0.125);
        this.elapsedTime = TimeSpan.FromSeconds(0);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 35, 35);
    }

    public void TakeDamage() { isDead = true; velocity = Vector2.Zero; }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;
    public void BounceX() { velocity.X *= -1; facingLeft = !facingLeft; }
    public void BounceY() { velocity.Y *= -1; }

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
        if (isDead) { currentFrame = 7; return; }

        elapsedTime += gameTime.ElapsedGameTime;
        if (elapsedTime >= delay)
        {
            elapsedTime -= delay;
            currentFrame = (currentFrame + 1) % 7;
        }

        position.X += velocity.X;
        if (position.X > 760 || position.X < 0)
        {
            velocity.X *= -1;
            facingLeft = velocity.X < 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, System.Numerics.Vector2 startCoords)
    {
        var direction = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        if (texture != null)
        {
            spriteBatch.Draw(texture, position, frames[currentFrame],
                Color.White, 0f, Vector2.Zero, 0.35f, direction, 0f);
        }
    }
}