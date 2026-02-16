using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public interface IPlayerState
{
    void Update(Player player, GameTime gameTime);
    void Draw(Player player);
    void Walk(Player player, int direction);
    void Jump(Player player);
    void Attack(Player player);
    void TakeDamage(Player player);
}

public class IdleState : IPlayerState
{
    public void Update(Player player, GameTime gameTime)
    {
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

    }
    public void Walk(Player player, int direction)
    {
        player.ChangeState(new WalkingState(direction));
    }
    public void Jump(Player player)
    {
        player.ChangeState(new JumpState());
    }
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState());
    }
    public void TakeDamage(Player player)
    {  
        player.ChangeState(new DamagedState());
    }
}

public class WalkingState : IPlayerState
{
    private float offsetX = 0f;
    private int direction = 1;

    public WalkingState(int direction)
    {
        this.direction = direction;
    }

    public void Update(Player player, GameTime gameTime)
    {
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

    }
    public void Walk(Player player, int direction)
    {
        if (direction > 0)
        {
            player.facing = SpriteEffects.None;
        }
        else if (direction < 0)
        {
            player.facing = SpriteEffects.FlipHorizontally;
        }
        player.Position.X += direction * 5f;
    }
    public void Jump(Player player)
    {
        player.ChangeState(new JumpState());
    }
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState());
    }

    public void TakeDamage(Player player)
    {
        player.ChangeState(new DamagedState());
    }
}

public class JumpState : IPlayerState
{
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
        player.Velocity.Y = -500f;
    }
    public void Update(Player player, GameTime gameTime)
    {
        // jump action
        player.Velocity.Y += 20f;
        player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // we decide 200 is the "floor"
        // this will change later when we implement collision detection etc
        if (player.Position.Y >= 200)
        {
            player.Position.Y = 200;
            player.ChangeState(new IdleState());
        }
    }
    public void Walk(Player player, int direction)
    {
        // allow slight changes in direction when jumping
        if (direction > 0)
        {
            player.facing = SpriteEffects.None;
        }
        else if (direction < 0)
        {
            player.facing = SpriteEffects.FlipHorizontally;
        }
        player.Position.X += direction * 3f;
    }
    public void Jump(Player player)
    {
        // do nothing, can't jump while jumping
    }
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState(wasJumping: true));
    }

    public void TakeDamage(Player player)
    {
        player.ChangeState(new DamagedState());
    }
}

public class AttackState : IPlayerState
{
    private float attackTimer = 0f;
    private const float attackDuration = 0.3f;
    private bool wasJumping = false;
    
    public AttackState(bool wasJumping = false)
    {
        this.wasJumping = wasJumping;
    }
    
    public void Update(Player player, GameTime gameTime)
    {
        attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Apply gravity if we were jumping
        if (wasJumping)
        {
            player.Velocity.Y += 20f;
            player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Check if landed
            if (player.Position.Y >= 200)
            {
                player.Position.Y = 200;
                player.ChangeState(new IdleState());
                return;
            }
        }
        
        if (attackTimer >= attackDuration)
        {
            if (wasJumping)
                player.ChangeState(new JumpState());
            else
                player.ChangeState(new IdleState());
        }
    }
    
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Attack"];
        int frameWidth = 128;
        int frameHeight = 128;
        int frameX = 896;
        int frameY = 0;
        player.sourceRectangle = new Rectangle(frameX, frameY, frameWidth, frameHeight);
    }
    
    public void Walk(Player player, int direction) 
    {
        if (wasJumping)
        {
            if (direction > 0)
                player.facing = SpriteEffects.None;
            else if (direction < 0)
                player.facing = SpriteEffects.FlipHorizontally;
            player.Position.X += direction * 3f;
        }
    }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) 
    {
        player.ChangeState(new DamagedState());
    }
}

public class DamagedState : IPlayerState
{
    private float damageTimer = 0f;
    private const float damageDuration = 0.5f;
    private float blinkTimer = 0f;
    
    public void Update(Player player, GameTime gameTime)
    {
        damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Toggle between red and transparent for flashing
        if (blinkTimer >= 0.1f)
        {
            player.DrawColor = (player.DrawColor == Color.Red) ? Color.Transparent : Color.Red;
            blinkTimer = 0f;
        }
        
        if (damageTimer >= damageDuration)
        {
            player.DrawColor = Color.White; // Reset color
            player.ChangeState(new IdleState());
        }
    }
    
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }
    
    public void Walk(Player player, int direction) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) { }
}