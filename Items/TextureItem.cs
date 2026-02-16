using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A game item that displays a texture with a gentle floating animation.
/// Used for Hollow Knight charm/item pickups loaded from Content.
/// </summary>
public class TextureItem : IItem
{
    private readonly Texture2D texture;
    private float timer;

    public TextureItem(Texture2D texture)
    {
        this.texture = texture;
        timer = 0f;
    }

    public void Update(GameTime gameTime)
    {
        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        // Gentle floating bob animation
        float bob = (float)Math.Sin(timer * 2.5) * 8f;
        Vector2 drawPos = new Vector2(
            position.X - texture.Width / 2f,
            position.Y - texture.Height / 2f + bob
        );

        spriteBatch.Draw(texture, drawPos, Color.White);
    }

    public void Reset()
    {
        timer = 0f;
    }
}
