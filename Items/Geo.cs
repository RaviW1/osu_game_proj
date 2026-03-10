using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Geo
{
    private Texture2D texture;
    private Vector2 position;
    public bool IsCollected { get; private set; }

    private const float DrawScale = 0.35f;
    private const int BoundsWidth = 10;
    private const int BoundsHeight = 10;

    public Geo(Texture2D texture, Vector2 position)
    {
        this.texture = texture;
        this.position = position;
        IsCollected = false;
    }

    public void Update(GameTime gameTime)
    {
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)position.X, (int)position.Y,
            BoundsWidth, BoundsHeight);
    }

    public void Collect()
    {
        IsCollected = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsCollected) return;

        spriteBatch.Draw(texture, position, null, Color.White,
            0f, Vector2.Zero, DrawScale, SpriteEffects.None, 0f);
    }
}
