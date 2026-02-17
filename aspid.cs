using System;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class Aspid : ISprite
{
    private Texture2D texture;
    private Texture2D fireballTexture;
    private Vector2 position;
    private Vector2 velocity;
    private float hoverTimer = 0f;
    private float shootTimer = 0f;
    private float shootInterval = 2f; // Shoot every 2 seconds
    private bool facingLeft = true;
    
    public List<Projectile> Projectiles { get; private set; }
    
    public Aspid(Texture2D texture, Texture2D fireballTexture, Vector2 startPosition)
    {
        this.texture = texture;
        this.fireballTexture = fireballTexture;
        this.position = startPosition;
        this.velocity = new Vector2(-30, 30); 
        this.Projectiles = new List<Projectile>();
    }
    
    public void Update()
    {
        
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
    
    private void ShootFireball()
    {
        Vector2 fireballVelocity = facingLeft ? new Vector2(-150, 0) : new Vector2(150, 0);
        Projectile fireball = new Projectile(fireballTexture, position, fireballVelocity);
        Projectiles.Add(fireball);
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