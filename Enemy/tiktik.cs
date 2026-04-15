using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class Tiktik : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private bool facingLeft;
    private int currentFrame;
    private int maxFrame;
    private Rectangle[] frames = new Rectangle[6];
    private Rectangle[] patrolFrames = new Rectangle[4];
    private Rectangle[] damageFrames = new Rectangle[2];
    private Rectangle[] deathFrames = new Rectangle[6];
    private TimeSpan delay;
    private TimeSpan elapsedTime;

    public Tiktik (Texture2D texture, Vector2 startPosition) {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2((float)0.6, 0);
        this.facingLeft = false;
        this.delay = TimeSpan.FromSeconds(0.125);
        this.elapsedTime = TimeSpan.FromSeconds(0);

        for (int i = 0; i < 4; i++)
        {
            this.patrolFrames[i] = new Rectangle(4 + 96 * i, 23, 91, 78);
        }
        for (int i = 0; i < 2; i++)
        {
            this.damageFrames[i] = new Rectangle(4 + 100 * i, 125, 95, 79);
        }
        for (int i = 0; i < 4; i++)
        {
            this.deathFrames[i] = new Rectangle(4 + 98 * i, 228, 93, 91);
        }
        for (int i = 0; i < 2; i++)
        {
            this.deathFrames[i + 4] = new Rectangle(4 + 105 * i, 343, 100, 71);
        }
        this.frames = patrolFrames;
        this.currentFrame = 0;
        this.maxFrame =this.frames.Length;
    }

    public void Patrol()
    {
        this.frames = this.patrolFrames;
        this.currentFrame = 0;
        this.maxFrame = this.frames.Length;
    }

    public void TakeDamage()
    {
        this.frames = this.damageFrames;
        this.currentFrame = 0;
        this.maxFrame = this.frames.Length;
    }

    public void Die()
    {
        this.frames = this.deathFrames;
        this.currentFrame = 0;
        this.maxFrame = this.frames.Length;
    }

    public bool IsDead => false;
    public Rectangle GetBounds() { return new Rectangle(0, 0, 0, 0); }
    public float GetVelocityX() { return 0; }
    public float GetVelocityY() { return 0; }
    public void ResolveCollisions(List<CollisionResult> results) { }

    public void Update(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime;
        if (elapsedTime >= delay)
        {
            elapsedTime -= delay;
            currentFrame = (currentFrame + 1) % maxFrame;
        }

        this.position.X += this.velocity.X;
        if (this.position.X > 760 || this.position.X < 0)
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
            spriteBatch.Draw(this.texture, this.position, this.frames[this.currentFrame],
                Color.White, 0f, Vector2.Zero, 0.35f, direction, 0f);
        }
    }
}