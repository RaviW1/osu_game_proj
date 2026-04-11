using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MapBlock : ISprite
{
    private Texture2D texture;
    public Vector2 position;

    public MapBlock(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        spriteBatch.Draw(texture, position, null, Color.White,
            0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
    }
}
