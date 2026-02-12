using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework;

public class Player
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Dictionary<string, Texture2D> Textures;
    public Texture2D CurrentTexture;
    public Rectangle sourceRectangle;

    private IPlayerState currentState;

    public Player(Dictionary<string, Texture2D> textures)
    {
        Textures = textures;
        currentState = new IdleState();
        currentState.Draw(this);
    }

    public void Update()
    {

    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startCoords)
    {
        if (CurrentTexture != null)
        {
            spriteBatch.Draw(CurrentTexture, startCoords, sourceRectangle, Color.White);
        }

    }
}
