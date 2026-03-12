using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;

public class HuskBully : ISprite
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private bool facingLeft = true;
    private bool isDead = false;
    private float deathVelocityY = 0f;
    private const float floorY = 400f;
    private int currentFrame;
    
    public bool IsDead => isDead;
    
    // Constructor
    public HuskBully(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(50, 0);
        this.currentFrame = 0;
    }

    public Rectangle GetBounds(){
        return new Rectangle((int)position.X, (int)position.Y, 56, 64); 
    }
    public void TakeDamage(){
        this.isDead = true;
        this.velocity = Vector2.Zero;
    }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;
    public void Update()
    {
        if (this.isDead){
            deathVelocityY += 20f;
            position.Y += deathVelocityY * 0.016f;
            if (position.Y >= floorY)
                position.Y = floorY;
            return;
        }
        position.X += velocity.X * 0.016f;
        
        if (position.X > 700 || position.X < 100)
        {
            velocity.X *= -1;
        }
        
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords){
        Vector2 drawPos = new Vector2(position.X, position.Y);
    
        var xnaDrawPos = new Vector2(drawPos.X, drawPos.Y);
    
        if (texture != null)
        {
        
        int frameWidth = 280;   
        int frameHeight = 320;  
        int frameX = 868;       
        int frameY = 70;         
        
        var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);
        
        // Scale it to reasonable size
        float scale = 0.2f;
        
        spriteBatch.Draw(texture, xnaDrawPos, sourceRect, Microsoft.Xna.Framework.Color.White,
                        0f, Microsoft.Xna.Framework.Vector2.Zero, scale,
                        Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}