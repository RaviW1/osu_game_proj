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
    private bool facingLeft;
    private bool isDead;
    private float floorY = 400f;
    private int currentFrame;
    private Rectangle[] frames = new Rectangle[8];
    private TimeSpan delay = TimeSpan.FromSeconds(0.125);
    private TimeSpan elapsedTime;

    public bool IsDead => isDead;
    
    // Constructor
    public HuskBully(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = new Vector2(50, 0);
        this.facingLeft = true;
        this.isDead = false;
        this.floorY = 400f;

        // Sprite setup
        this.currentFrame = 0;
        for (int i = 0; i < 7; i++)
        {
            this.frames[i] = new Rectangle(3 + 111*i, 174, 107, 128);
        }
        this.frames[7] = new Rectangle(492, 1165, 159, 110);
    }

    public Rectangle GetBounds(){
        return new Rectangle((int)position.X, (int)position.Y, 159, 128); 
    }

    public void TakeDamage(){
        this.isDead = true;
        this.velocity = Vector2.Zero;
    }

    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;

    public void Update(GameTime gameTime, GameWindow window)
    {
        // Check if enemy is dead
        if (this.isDead){
            this.currentFrame = 7;
            return;
        } else
        {
            this.elapsedTime += gameTime.ElapsedGameTime;
            if (this.elapsedTime >= this.delay)
            {
                this.elapsedTime -= this.delay;

                // change animation frame
                this.currentFrame = this.currentFrame % 7;
            }

        // Otherwise continue walking around
        position.X += velocity.X * 0.016f;
        
        if (position.X > 700 || position.X < 100)
        {
            velocity.X *= -1;
            this.facingLeft = velocity.X < 0; // Update facing direction
        }
        
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords){
        // Scale to a reasonable size
        float scale = 0.2f;

        // Check direction
        var direction = SpriteEffects.None;
        if (!this.facingLeft)
        {
            direction = SpriteEffects.FlipHorizontally;
        }

        if (texture != null)
        {        
        spriteBatch.Draw(
            this.texture,               // texture
            this.position,              // position
            this.frames[currentFrame],  // sourceRectangle
            Color.White,                // color
            0f,                         // rotation
            Vector2.Zero,               // origin
            scale,                      // scale
            direction,                  // effects
            0f                          // layerDepth
            );
        }
    }
}