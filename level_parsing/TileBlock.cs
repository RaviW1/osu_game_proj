using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class TileBlock
{
    private Texture2D texture;
    private Rectangle destRect;
    public Rectangle bounds;
    public bool isCollideable;
    // TODO: create field to implement hazards

    public TileBlock(Texture2D texture, Rectangle destRectangle, bool isCollideable)
    {
        this.texture = texture;
        this.destRect = destRectangle;
        this.bounds = destRectangle;
        this.isCollideable = isCollideable;
    }

    public void Update()
    {
        // Question: Do we need this for blocks? - Brooklynn
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            this.texture,                           // texture
            this.destRect,                          // position
            null,                                   // sourceRectangle
            Microsoft.Xna.Framework.Color.White,    // color
            0.0f,                                   // rotation
            new Vector2(0, 0),                      // origin
            SpriteEffects.None,                     // effects
            0.0f                                    // layerDepth
            );
    }
}
