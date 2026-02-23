using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IItem
{
    int Id { get; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, bool isSelected);
    void Reset();
    void OnSelect(Player player);
    void OnDeselect(Player player);
}
