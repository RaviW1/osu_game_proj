using System;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public class Boofly : ISprite
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private float bobTimer = 0f;
    private bool isDead = false;
    private float deathVelocityY = 0f;
    private const float floorY = 400f;

    public bool IsDead => isDead;
    
    public Boofly(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(50, 0);
    }

    public Microsoft.Xna.Framework.Rectangle GetBounds(){
        return new Microsoft.Xna.Framework.Rectangle(
            (int)position.X, (int)position.Y, 56, 64); 
    }
    public void TakeDamage(){
        isDead = true;
        velocity = Vector2.Zero;
    }
    
    public void Update()
    {
        if (isDead){
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
        
        bobTimer += 0.016f;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords){
    float bobOffset = (float)Math.Sin(bobTimer * 3) * 20;
    Vector2 drawPos = new Vector2(position.X, position.Y + bobOffset);
    
    var xnaDrawPos = new Microsoft.Xna.Framework.Vector2(drawPos.X, drawPos.Y);
    
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