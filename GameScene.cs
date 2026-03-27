using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

public class GameScene : IScene
{
    private readonly GraphicsDevice _graphics;
    private readonly ContentManager _content;

    public GameScene(GraphicsDevice graphics, ContentManager content)
    {
        _graphics = graphics;
        _content = content;
    }

    public void Load() { }
    public void Update(GameTime gameTime) { }
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
    public void Unload() { }
}