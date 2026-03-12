using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface ISprite
{
    void Update(GameTime gameTime, GameWindow window);
    void Draw(SpriteBatch spriteBatch, System.Numerics.Vector2 startCoords);
}
