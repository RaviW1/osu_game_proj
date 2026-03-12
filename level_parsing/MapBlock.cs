using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public class MapBlock : ISprite
{
    private Texture2D texture;
    public System.Numerics.Vector2 position;

    public MapBlock(Texture2D texture, System.Numerics.Vector2 startPosition)
    {
        this.texture = texture;
        this.position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, System.Numerics.Vector2 startCoords)
    {
        spriteBatch.Draw(
            this.texture,                           // texture
            this.position,                          // position
            null,                                   // sourceRectangle
            Microsoft.Xna.Framework.Color.White,    // color
            0.0f,                                   // rotation
            new System.Numerics.Vector2(0, 0),      // origin
            1.0f,                                   // scale
            SpriteEffects.None,                     // effects
            0.0f                                    // layerDepth
            );
    }
}
