using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using osu_game_proj;


public class Husk_Bully : ISprite
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private bool facingLeft = true;
    private bool isDead = false;
    private float deathVelocityY = 0f;
    private const float floorY = 400f;

    public bool IsDead => isDead;

    public Husk_Bully(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(-30, 30);
    }
    public Rectangle GetBounds()
    {
        return new Microsoft.Xna.Framework.Rectangle(
        (int)position.X, (int)position.Y, 45, 60);
    }
    public void TakeDamage()
    {
        isDead = true;
        velocity = Vector2.Zero;
        Projectiles.Clear();
    }

    public void Update()
    {
        if (isDead)
        {
            deathVelocityY += 20f;
            position.Y += deathVelocityY * 0.016f;
            if (position.Y >= floorY)
                position.Y = floorY;
            return;
        }

        position.X += velocity.X * 0.016f;
        position.Y += velocity.Y * 0.016f;

        // Bounce off edges and flip direction
        if (position.X > 700 || position.X < 100)
        {
            velocity.X *= -1;
            facingLeft = velocity.X < 0; // Update facing direction
        }
        if (position.Y > 400 || position.Y < 50)
            velocity.Y *= -1;

        hoverTimer += 0.016f;

        // Shooting logic
        shootTimer += 0.016f;
        if (shootTimer >= shootInterval)
        {
            ShootFireball();
            shootTimer = 0f;
        }

        // Update projectiles
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update();

            // Remove off-screen projectiles
            var projPos = Projectiles[i].GetPosition();
            if (projPos.X < -50 || projPos.X > 850 || projPos.Y < -50 || projPos.Y > 650)
            {
                Projectiles.RemoveAt(i);
            }
        }
    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        float hoverOffset = (float)Math.Sin(hoverTimer * 5) * 10;
        Vector2 drawPos = new Vector2(position.X, position.Y + hoverOffset);

        var xnaDrawPos = new Microsoft.Xna.Framework.Vector2(drawPos.X, drawPos.Y);

        if (texture != null)
        {
            int frameWidth = 90;
            int frameHeight = 120;
            int frameX = 0;
            int frameY = 320;

            var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);

            float scale = 0.5f;

            // Flip sprite based on facing direction
            var spriteEffect = facingLeft ?
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None :
                Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, xnaDrawPos, sourceRect, Microsoft.Xna.Framework.Color.White,
                            0f, Microsoft.Xna.Framework.Vector2.Zero, scale,
                            spriteEffect, 0f);
        }

        // Draw projectiles
        foreach (var projectile in Projectiles)
        {
            projectile.Draw(spriteBatch, Vector2.Zero);
        }
    }
}
