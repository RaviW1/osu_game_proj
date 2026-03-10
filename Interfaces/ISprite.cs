using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public interface ISprite
{
    void Update();
    void Draw(SpriteBatch spriteBatch, Vector2 startCoords);
}
