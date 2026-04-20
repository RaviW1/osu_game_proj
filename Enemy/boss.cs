using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Boss : ISprite
{
    private Texture2D texture;
    private Vector2 position;
    private bool isDead;
    private int currentFrame;

    public bool IsDead => isDead;

    public Boss(Texture2D texture, Vector2 startPos)
    {
        this.texture = texture;
        this.position = startPos;
        this.isDead = false;
        this.currentFrame = 0;
    }

    public void Update(GameTime gameTime)
    {

    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startPos)
    {
    }
    public Rectangle GetBounds()
    {

        return new Rectangle();
    }
}
