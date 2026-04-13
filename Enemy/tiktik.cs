using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Tiktik : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
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
    }

    public void Patrol()
    {
        this.frames = this.patrolFrames;
    }

    public void TakeDamage()
    {
        this.frames = this.damageFrames;
    }

    public void Die()
    {
        this.frames = this.deathFrames;
    }

    public void Update(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime;
        if (elapsedTime >= delay)
        {
            elapsedTime -= delay;
            currentFrame = (currentFrame + 1) % maxFrame;
        }
    }
}