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

    private float invincibilityTimer = 0f;
    private const float InvincibilityDuration = 1.0f;
    private const float GravityForce = 1200f;

    // Dash
    public float DashTimer = 0f;
    public float DashCooldown = 0f;
    public bool HasAirDash = false;
    public bool IsDashing = false;
    public const float DashDuration = 0.18f;
    public const float DashSpeed = 800f;
    public const float DashCooldownDuration = 3f;

    public bool IsInvincible => invincibilityTimer > 0f || IsDashing;
    public bool SuppressLandingTransition { get; set; } = false;
    public bool IsAttacking { get; set; } = false;
    public bool OnGround { get; set; }
    public bool IsAirborne { get; set; } = false;

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

    public Player(Dictionary<string, Texture2D> textures, Texture2D fireballTexture, Vector2 startCoords)
    {
        Textures = textures;
        this.fireballTexture = fireballTexture;
        this.Projectiles = new List<Projectile>();
        Position = startCoords;
        currentState = new IdleState();
        currentState.OnEnter(this);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (invincibilityTimer > 0f)
            invincibilityTimer -= dt;

        if (DashCooldown > 0f)
            DashCooldown -= dt;

        // Recharge air dash on landing
        if (OnGround)
            HasAirDash = true;

        // Gravity — suppressed while dashing
        if (!OnGround && !IsDashing)
        {
            Velocity.Y += GravityForce * dt;
            Position.Y += Velocity.Y * dt;
        }

        currentState.Update(this, gameTime);

        if (currentState is WalkingState)
            SoundManager.StartWalkingSound();
        else
            SoundManager.StopWalkingSound();

        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            Projectiles[i].Update(gameTime);
            var projPos = Projectiles[i].GetPosition();
            if (projPos.X < -50 || projPos.X > 850)
                Projectiles.RemoveAt(i);
        }
    }

    public void ResolveCollisions(List<CollisionResult> results)
    {
        OnGround = false;

        foreach (var result in results)
        {
            if (result.IsHarmful && !IsInvincible)
            {
                PlayerHealth--;
                TakeDamage();
            }

            if (!result.IsCollideable) continue;

            switch (result.Direction)
            {
                case CollisionDirection.Down:
                    if (Velocity.Y < 0) break;
                    Position.Y -= result.Overlap.Height - 1;
                    Velocity.Y = 0;
                    IsAirborne = false;
                    OnGround = true;
                    break;

                case CollisionDirection.Up:
                    Position.Y += result.Overlap.Height;
                    Velocity.Y = 0;
                    break;

                case CollisionDirection.Left:
                    Position.X += result.Overlap.Width;
                    Velocity.X = 0;
                    break;

                case CollisionDirection.Right:
                    Position.X -= result.Overlap.Width;
                    Velocity.X = 0;
                    break;
            }
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState = newState;
        newState.OnEnter(this);
    }

    public void ShootFireball()
    {
        float direction = (facing == SpriteEffects.FlipHorizontally) ? -1 : 1;
        System.Numerics.Vector2 fireballVelocity = new System.Numerics.Vector2(direction * 200, 0);
        float bodyOffsetY = -sourceRectangle.Height / 4f;
        System.Numerics.Vector2 startPos = new System.Numerics.Vector2(Position.X, Position.Y + bodyOffsetY);
        Projectiles.Add(new Projectile(fireballTexture, startPos, fireballVelocity));
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        currentState.Draw(this);
        //NOTE:  changed from height / 2 
        // if there are future issues with drawing, this may be the culprit
        Vector2 origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height);
        spriteBatch.Draw(CurrentTexture, Position, sourceRectangle, DrawColor, 0f, origin, 0.5f, facing, 0f);

        foreach (var projectile in Projectiles)
            projectile.Draw(spriteBatch, System.Numerics.Vector2.Zero);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)(Position.X - 15),
            (int)(Position.Y - 40),
            30, 40);
    }

    public Rectangle GetMeleeHitbox()
    {
        int hitboxWidth = 40;
        int hitboxHeight = 30;
        int offsetX = (facing == SpriteEffects.FlipHorizontally) ? -hitboxWidth - 15 : 15;
        return new Rectangle(
            (int)(Position.X + offsetX),
            (int)(Position.Y - 20),
            hitboxWidth, hitboxHeight);
    }

    public void JumpHeld(float deltaTime) => currentState.JumpHeld(this, deltaTime);
    public void Walk(int direction)
    {
        if (direction == 0)
            currentState.StopWalking(this);
        else
            currentState.Walk(this, direction);
    }
    public void Jump() => currentState.Jump(this);
    public void Attack() => currentState.Attack(this);
    public void Heal() => currentState.Heal(this);
    public void Dash() => currentState.Dash(this);
    public void LookUp() => currentState.LookUp(this);

    public void TakeDamage()
    {
        if (IsInvincible) return;
        invincibilityTimer = InvincibilityDuration;
        currentState.TakeDamage(this);
    }

    // Shared dash guard — called by any state that supports dashing
    public bool TryDash()
    {
        if (!CanDash) return false;
        if (DashCooldown > 0f) return false;
        if (IsAirborne && !HasAirDash) return false;
        return true;
    }
}
