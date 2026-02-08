using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

public interface IPlayer
{   
   void Update();
   void Draw(SpriteBatch spriteBatch, Vector2 startCoords);
}
public class Player  : IPlayer
{
    public void Update()
    {

    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {

    }
}