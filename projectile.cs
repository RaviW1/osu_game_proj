using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public class Projectile : ISprite
{
    private Vector2 position;
    private Vector2 velocity;
    private Texture2D texture;
    private bool movingRight;
    
    // Animation fields
    private int currentFrame = 0;
    private int totalFrames = 4;
    private float animationTimer = 0f;
    private float frameTime = 0.1f; // 0.1 seconds per frame
    
    public Projectile(Texture2D texture, Vector2 startPos, Vector2 velocity)
    {
        this.texture = texture;
        this.position = startPos;
        this.velocity = velocity;
        this.movingRight = velocity.X > 0;
    }
    
    public void Update()
    {
        position += velocity * 0.016f;
        
        // Update animation
        animationTimer += 0.016f;
        if (animationTimer >= frameTime)
        {
            currentFrame = (currentFrame + 1) % totalFrames; // Cycle 0->1->2->3->0
            animationTimer = 0f;
        }
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
            int frameWidth = texture.Width / 2;
            int frameHeight = texture.Height / 2;
            
            // Calculate frame position (2x2 grid)
            int frameX = (currentFrame % 2) * frameWidth;
            int frameY = (currentFrame / 2) * frameHeight;
            
            var sourceRect = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, frameWidth, frameHeight);
            
            // Flip sprite if moving right
            var spriteEffect = movingRight ? 
                Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : 
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            
            spriteBatch.Draw(texture, xnaPos, sourceRect, Microsoft.Xna.Framework.Color.White, 
                           0f, Microsoft.Xna.Framework.Vector2.Zero, 0.5f,
                           spriteEffect, 0f);
        }
    }
}