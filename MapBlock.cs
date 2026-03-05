using System.Numerics;
using Microsoft.Xna.Framework.Graphics;


public class MapBlock : ISprite
{
    private Texture2D texture;
    private Vector2 position;

    public MapBlock(Texture2D texture, Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
    }

    public void Update()
    {
        // Question: Do we need this for blocks? - Brooklynn
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        spriteBatch.Draw(
            this.texture,                           // texture
            this.position,                          // position
            null,                                   // sourceRectangle
            Microsoft.Xna.Framework.Color.White,    // color
            0.0f,                                   // rotation
            new Vector2(0, 0),                      // origin
            1.0f,                                   // scale
            SpriteEffects.None,                     // effects
            0.0f                                    // layerDepth
            );
    }
}