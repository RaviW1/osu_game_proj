using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public class Projectile : ISprite
{
    private Vector2 position;
    private Vector2 velocity;
    private Texture2D texture;
    
    public Projectile(Texture2D texture, Vector2 startPos, Vector2 velocity)
    {
        this.texture = texture;
        this.position = startPos;
        this.velocity = velocity;
    }
    
    public void Update()
    {
        position += velocity * 0.016f;
    }
    
    public Vector2 GetPosition()
    {
        return position;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        var xnaPos = new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        if (texture != null)
        {
            // Draw just one frame from the sprite sheet
            
            int frameWidth = texture.Width / 2;
            int frameHeight = texture.Height / 2;
            int frameX = 0;  
            int frameY = 0;  
        
            var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);
        
            
            spriteBatch.Draw(texture, xnaPos, sourceRect, Microsoft.Xna.Framework.Color.White, 
                       0f, Microsoft.Xna.Framework.Vector2.Zero, 0.5f, 
                       Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }
    }
}