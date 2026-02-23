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
    public List<Projectile> Projectiles { get; private set; }
    private Texture2D fireballTexture;


    public Player(Dictionary<string, Texture2D> textures, Texture2D fireballTexture, Vector2 startCoords)
    {
        Textures = textures;
        this.fireballTexture = fireballTexture;
        this.Projectiles = new List<Projectile>();
        Position = startCoords;
        currentState = new IdleState();
        currentState.Reset(this);
    }

    public void Update(GameTime gameTime)
    {
        currentState.Update(this, gameTime);

        // Update projectiles
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update();

            // Remove off-screen projectiles
            var projPos = Projectiles[i].GetPosition();
            if (projPos.X < -50 || projPos.X > 850)
            {
                Projectiles.RemoveAt(i);
            }
        }
    }
    public void ChangeState(IPlayerState newState)
    {
        currentState = newState;
        newState.Reset(this);
    }
    public void ShootFireball()
    {
        // Shoot in the direction player is facing
        float direction = (facing == SpriteEffects.FlipHorizontally) ? -1 : 1;
        System.Numerics.Vector2 fireballVelocity = new System.Numerics.Vector2(direction * 200, 0);

        float bodyOffsetY = -sourceRectangle.Height / 4f;
        System.Numerics.Vector2 startPos = new System.Numerics.Vector2(Position.X, Position.Y + bodyOffsetY);

        Projectile fireball = new Projectile(fireballTexture, startPos, fireballVelocity);

        Projectiles.Add(fireball);
    }
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        currentState.Draw(this);
        Vector2 origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height / 2f);
        spriteBatch.Draw(CurrentTexture, Position, sourceRectangle, DrawColor, 0f, origin, 0.5f, facing, 0f); // Use DrawColor instead of Color.White

        foreach (var projectile in Projectiles)
        {
            projectile.Draw(spriteBatch, System.Numerics.Vector2.Zero);
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
    public void Heal()
    {
        currentState.Heal(this);
    }
}
