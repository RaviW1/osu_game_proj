using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IItem
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position);
    void Reset();
}
