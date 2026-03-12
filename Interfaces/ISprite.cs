using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface ISprite
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, System.Numerics.Vector2 startCoords);
}
