using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HuskBully : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private bool facingLeft;
    private bool isDead;
    private bool isPhased;
    private int currentFrame;
    private Rectangle[] frames = new Rectangle[8];
    private TimeSpan delay;
    private TimeSpan elapsedTime;
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
    public bool IsPhased => isPhased;
    public bool ShouldBeRemoved => isDead && deathTimer >= DeathRemovalDelay;
    public int Health => health;
    public int MaxHealth => maxHealth;

    public HuskBully(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(-1, 0);
        this.facingLeft = true;
        this.isDead = false;
        this.isPhased = false;
        this.currentFrame = 0;
        this.patrolLeft = startPosition.X - 150f;
        this.patrolRight = startPosition.X + 150f;

        for (int i = 0; i < 7; i++)
            this.frames[i] = new Rectangle(4 + 111 * i, 175, 106, 128);
        this.frames[7] = new Rectangle(492, 1165, 159, 110);

        this.delay = TimeSpan.FromSeconds(0.125);
        this.elapsedTime = TimeSpan.FromSeconds(0);
        this.maxHealth = BaseHealth * osu_game_proj.Difficulty.HpMultiplier;
        this.health = this.maxHealth;
    }

    public Rectangle GetBounds()
    {
        // Only suppress hitbox during i-frames while ALIVE
        // Dead enemies always return real bounds so physics collision still works
        if (!isDead && invincibilityTimer > 0f) return Rectangle.Empty;
        return new Rectangle((int)position.X, (int)position.Y, 35, 35);
    }

    public void TakeDamage()
    {
        if (isPhased || isDead || invincibilityTimer > 0f) return;
        health--;
        invincibilityTimer = InvincibilityDuration;
        if (health <= 0)
        {
            isDead = true;
            velocity = Vector2.Zero;
        }
    }

    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;
    public void BounceX() { velocity.X *= -1; facingLeft = !facingLeft; }
    public void BounceY() { velocity.Y *= -1; }

    public void ResolveCollisions(List<CollisionResult> results)
    {
        bool touchingSpike = false;
        foreach (var result in results)
        {
            if (result.IsHarmful)
            {
                touchingSpike = true;
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
        isPhased = touchingSpike;
    }

    public void Update(GameTime gameTime)
    {
        if (isDead)
        {
            currentFrame = 7;
            deathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            return;
        }

        if (invincibilityTimer > 0f) invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        elapsedTime += gameTime.ElapsedGameTime;
        if (elapsedTime >= delay)
        {
            elapsedTime -= delay;
            currentFrame = (currentFrame + 1) % 7;
        }

        position.X += velocity.X;

        if (position.X > patrolRight || position.X < patrolLeft)
        {
            velocity.X *= -1;
            facingLeft = velocity.X < 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        var direction = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        if (texture != null)
        {
            Color baseTint = isPhased ? Color.White * 0.4f : Color.White;
            Color tint = GetDeathTint(baseTint);
            spriteBatch.Draw(texture, position, frames[currentFrame], tint, 0f, Vector2.Zero, 0.35f, direction, 0f);
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