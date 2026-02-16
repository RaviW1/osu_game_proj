using System;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public class Aspid : ISprite
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private float hoverTimer = 0f;
    
    public Aspid(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(30, 30); 
    }
    
    public void Update()
    {
        
        position.X += velocity.X * 0.016f;
        position.Y += velocity.Y * 0.016f;
        
        
        if (position.X > 700 || position.X < 100)
            velocity.X *= -1;
        if (position.Y > 400 || position.Y < 50)
            velocity.Y *= -1;
        
        hoverTimer += 0.016f;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords){
        float hoverOffset = (float)Math.Sin(hoverTimer * 5) * 10;
        Vector2 drawPos = new Vector2(position.X, position.Y + hoverOffset);
    
        var xnaDrawPos = new Microsoft.Xna.Framework.Vector2(drawPos.X, drawPos.Y);
    
        if (texture != null){
        
            int frameWidth = 90;   
            int frameHeight = 120;
            int frameX = 0;       
            int frameY = 320;        
        
            var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);
        
            float scale = 0.5f; 
        
            spriteBatch.Draw(texture, xnaDrawPos, sourceRect, Microsoft.Xna.Framework.Color.White,
                        0f, Microsoft.Xna.Framework.Vector2.Zero, scale,
                        Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}