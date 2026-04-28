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
    private float deathTimer = 0f;
    private const float DeathFlashStart = 3f;
    private const float DeathFlashDuration = 0.6f;
    private const float DeathRemovalDelay = 3.6f;
    private const int BaseHealth = 1;
    private int health;
    private int maxHealth;
    private float invincibilityTimer = 0f;
    private const float InvincibilityDuration = 0.3f;

    private float patrolLeft;
    private float patrolRight;

    public bool IsDead => isDead;
    public bool IsPhased => false;
    public bool ShouldBeRemoved => isDead && deathTimer >= DeathRemovalDelay;
    public int Health => health;
    public int MaxHealth => maxHealth;

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
        // Only suppress hitbox during i-frames while ALIVE
        // Dead enemies always return real bounds so physics collision still works
        if (!isDead && invincibilityTimer > 0f) return Rectangle.Empty;
        return new Rectangle((int)position.X, (int)position.Y, 56, 64);
    }

    public void BounceX() { velocity.X *= -1; }
    public void BounceY() { velocity.Y *= -1; }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;

    public void TakeDamage()
    {
        if (isDead || invincibilityTimer > 0f) return;
        health--;
        invincibilityTimer = InvincibilityDuration;
        if (health <= 0)
        {
            isDead = true;
            velocity = Vector2.Zero;
        }
    }

    public void ResolveCollisions(List<CollisionResult> results)
    {
        foreach (var result in results)
        {
            if (!isDead && result.IsHarmful)
            {
                TakeDamage();
                continue;
            }
            if (!result.IsCollideable) continue;
            switch (result.Direction)
            {
                case CollisionDirection.Left:
                case CollisionDirection.Right:
                    if (!isDead) BounceX();
                    else position.X += (result.Direction == CollisionDirection.Left) ? result.Overlap.Width : -result.Overlap.Width;
                    break;
                case CollisionDirection.Down:
                    if (isDead)
                    {
                        position.Y -= result.Overlap.Height;
                        deathVelocityY = 0;
                        velocity = Vector2.Zero;
                    }
                    else BounceY();
                    break;
                case CollisionDirection.Up:
                    if (!isDead) BounceY();
                    break;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (isDead)
        {
            deathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            deathVelocityY += 600f * 0.016f;
            velocity = new Vector2(0, deathVelocityY);
            position.Y += velocity.Y * 0.016f;
            return;
        }

        if (invincibilityTimer > 0f) invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        position.X += velocity.X * 0.016f;

        if (position.X > patrolRight || position.X < patrolLeft)
            velocity.X *= -1;

        bobTimer += 0.016f;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        float bobOffset = (float)Math.Sin(bobTimer * 3) * 20;
        Vector2 drawPos = new Vector2(position.X, position.Y + bobOffset);

        if (texture != null)
        {
            var sourceRect = new Rectangle(4, 23, 309, 335);
            Color tint = GetDeathTint(Color.White);
            spriteBatch.Draw(texture, drawPos, sourceRect, tint, 0f, Vector2.Zero, 0.2f, SpriteEffects.None, 0f);
        }
    }

    private Color GetDeathTint(Color baseTint)
    {
        if (!isDead) return baseTint;
        float t = deathTimer - DeathFlashStart;
        if (t < 0f || t >= DeathFlashDuration) return baseTint;
        bool on = ((int)(t * 10)) % 2 == 0;
        return on ? baseTint : baseTint * 0.2f;
    }
}