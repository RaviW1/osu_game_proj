using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IScene
{
    void Load();
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    void Unload();
}