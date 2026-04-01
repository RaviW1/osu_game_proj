using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework;
using System;


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
    public List<TileBlock> Tiles { get; set; }

    private float invincibilityTimer = 0f;
    private const float InvincibilityDuration = 1.0f;
    public bool IsInvincible => invincibilityTimer > 0f;

    public bool SuppressLandingTransition { get; set; } = false;

    // Player status variables
    private int maxPlayerHealth = 7;
    private int playerHealth = 7;
    public int PlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth = Math.Min(value, maxPlayerHealth); }
    }
    public int MaxPlayerHealth
    {
        get { return maxPlayerHealth; }
        set
        {
            maxPlayerHealth = value;
            if (playerHealth > maxPlayerHealth)
                playerHealth = maxPlayerHealth;
        }
    }
    private Boolean canDash = false;
    public Boolean CanDash { get { return canDash; } set { canDash = value; } }
    private int soul = 0;
    public int Soul { get { return soul; } set { soul = value; } }
    private int soulLimit = 100;
    public int SoulLimit { get { return soulLimit; } set { soulLimit = value; } }
    public int geoCount = 0;
    public int GeoCount { get { return geoCount; } set { geoCount = value; } }

    public bool IsAttacking { get; set; } = false;
    public bool OnGround { get; set; }

    // Player movement variables
    public bool IsAirborne { get; set; } = false;
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

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (invincibilityTimer > 0f)
            invincibilityTimer -= dt;

        currentState.Update(this, gameTime);

        // Update projectiles
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update(gameTime);

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
    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)(Position.X - 15),
            (int)(Position.Y - 20),
            30, 40);
    }

    public Rectangle GetMeleeHitbox()
    {
        int hitboxWidth = 40;
        int hitboxHeight = 30;
        int offsetX = (facing == SpriteEffects.FlipHorizontally) ? -hitboxWidth - 15 : 15;
        return new Rectangle((int)(Position.X + offsetX),
            (int)(Position.Y - 20),
            hitboxWidth,
            hitboxHeight);
    }

    public void HandleOverlap(Rectangle tileBounds)
    {
        Rectangle playerBounds = new Rectangle(
            (int)(Position.X - 15),
            (int)(Position.Y - 20),
            30, 40);
        Rectangle overlap = Rectangle.Intersect(playerBounds, tileBounds);

        if (overlap.Width > overlap.Height)
        {
            // Player is above the tile and moving downward (landing) — not jumping through it
            if (Position.Y < tileBounds.Y && !SuppressLandingTransition)
            {
                Position.Y -= overlap.Height;
                Velocity.Y = 0;
                IsAirborne = false;
                ChangeState(new IdleState());
            }
            else if (Position.Y >= tileBounds.Y)
            {
                // Bonked a ceiling
                Position.Y += overlap.Height;
                Velocity.Y = 0;
            }
            // If SuppressLandingTransition is true and player is above tile, do nothing — let them pass through
        }
        else
        {
            if (playerBounds.X < tileBounds.X)
                Position.X -= overlap.Width;
            else
                Position.X += overlap.Width;

            Velocity.X = 0;
        }
    }

    public void JumpHeld(float deltaTime)
    {
        currentState.JumpHeld(this, deltaTime);
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
        if (IsInvincible) return;
        invincibilityTimer = InvincibilityDuration;
        currentState.TakeDamage(this);
    }
    public void Heal()
    {
        currentState.Heal(this);
    }
}
