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
    public SpriteEffects facing = SpriteEffects.None;

    private IPlayerState currentState;

    public Color DrawColor = Color.White;

    public Player(Dictionary<string, Texture2D> textures, Vector2 startCoords)
    {
        Textures = textures;
        Position = startCoords;
        currentState = new IdleState();
        currentState.Draw(this);
    }

    public void Update(GameTime gameTime)
    {
        currentState.Update(this, gameTime);

    }
    public void ChangeState(IPlayerState newState)
    {
        currentState = newState;
        newState.Draw(this);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        if (CurrentTexture != null)
        {
            Vector2 origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height / 2f);
            spriteBatch.Draw(CurrentTexture, Position, sourceRectangle, DrawColor, 0f, origin, 0.5f, facing, 0f); // Use DrawColor instead of Color.White
        }
    }
    public void Walk(int direction)
    {
        currentState.Walk(this, direction);
    }
    public void Jump()
    {
        currentState.Jump(this);
    }
    public void Attack()
    {
        currentState.Attack(this);
    }

    public void TakeDamage()
    {
        currentState.TakeDamage(this);
    }
}
