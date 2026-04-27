using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IBaldurState
{
    void OnEnter(BaldurBoss boss);
    void Update(BaldurBoss boss, GameTime gameTime);
    void Draw(BaldurBoss boss, SpriteBatch spriteBatch);
    Rectangle GetBounds(BaldurBoss boss);
}
