using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IBossState
{
    void OnEnter(Boss boss);
    void Update(Boss boss, GameTime gameTime);
    void Draw(Boss boss, SpriteBatch spriteBatch);
    void Run(Boss boss, int direction);

}
