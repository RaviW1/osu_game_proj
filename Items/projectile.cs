using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Projectile : ISprite
{
    private Vector2 position;
    private Vector2 velocity;
    private Texture2D texture;
    private bool movingRight;
    
    private int currentFrame = 0;
    private int totalFrames = 4;
    private float animationTimer = 0f;
    private float frameTime = 0.1f;
    
    public Projectile(Texture2D texture, Vector2 startPos, Vector2 velocity)
    {
        this.texture = texture;
        this.position = startPos;
        this.velocity = velocity;
        this.movingRight = velocity.X > 0;
    }
    
    public void Update(GameTime gameTime)
    {
        position += velocity * 0.016f;
        
        animationTimer += 0.016f;
        if (animationTimer >= frameTime)
        {
            currentFrame = (currentFrame + 1) % totalFrames;
            animationTimer = 0f;
        }
    }
    
    public Vector2 GetPosition()
    {
        return position;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        if (texture != null)
        {
            int frameWidth = texture.Width / 2;
            int frameHeight = texture.Height / 2;
            
            int frameX = (currentFrame % 2) * frameWidth;
            int frameY = (currentFrame / 2) * frameHeight;
            
            var sourceRect = new Rectangle(frameX, frameY, frameWidth, frameHeight);
            
            var spriteEffect = movingRight ? 
                SpriteEffects.FlipHorizontally : 
                SpriteEffects.None;
            
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 
                           0f, Vector2.Zero, 0.5f, spriteEffect, 0f);
        }
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 20, 20);
    }
}
