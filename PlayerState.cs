using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public interface IPlayerState
{
    void Update(Player player, GameTime gameTime);
    void Reset(Player player);
    void Draw(Player player);
    void Walk(Player player, int direction);
    void Jump(Player player);
    void Attack(Player player);
    void TakeDamage(Player player);

    void Heal(Player player);
}

public class IdleState : IPlayerState
{
    public void Update(Player player, GameTime gameTime)
    {
    }

    public void Reset(Player player)
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
    public void Heal(Player player)
    {
        player.ChangeState(new HealingState());
    }
    public void Attack(Player player)
    {
        player.ChangeState(new AttackState());
    }
    public void TakeDamage(Player player)
    {
        player.ChangeState(new DamagedState());
    }
    public void Draw(Player player) { }
}

public class WalkingState : IPlayerState
{
    private int direction = 1;
    private int currentFrame = 0;
    private int totalFrames = 8;
    private float timeSinceLastFrame = 0f;
    private bool commandReceivedThisFrame = true;
    private float secondsPerFrame = .1f;

    public WalkingState(int direction)
    {
        this.direction = direction;
        this.currentFrame = 0;
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (!commandReceivedThisFrame)
        {
            player.ChangeState(new IdleState());
            return;
        }

        commandReceivedThisFrame = false;

        timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;


        if (timeSinceLastFrame > secondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame++;
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
        }
    }
    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];

        int frameIndex = currentFrame % 9;
        int frameWidth = player.CurrentTexture.Width / 9;
        int frameHeight = player.CurrentTexture.Height;


        int xPosition = frameIndex * frameWidth;

        player.sourceRectangle = new Rectangle(xPosition, 0, frameWidth, frameHeight);
    }
    public void Walk(Player player, int direction)
    {
        commandReceivedThisFrame = true;
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
    public void Heal(Player player)
    {
        // Can't heal while walking - do nothing
    }
}

public class JumpState : IPlayerState
{
    private int currentFrame = 0;
    private int totalFrames = 12;
    private float timeSinceLastFrame = 0f;
    private float secondsPerFrame = .1f;

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];

        // grab idle sprite from walking animation
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 11 - 30, player.CurrentTexture.Height);
        player.Velocity.Y = -500f;
    }
    public void Update(Player player, GameTime gameTime)
    {
        // jump action
        player.Velocity.Y += 20f;
        player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;


        if (timeSinceLastFrame > secondsPerFrame)
        {
            timeSinceLastFrame = 0f;
            currentFrame++;
            if (currentFrame > totalFrames)
            {
                currentFrame = 0;
            }
        }

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
    public void Heal(Player player)
    {
        // Can't heal while moving/jumping/attacking/damaged - do nothing
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
    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Jumping"];
        int frameIndex = currentFrame % 12;
        int frameWidth = player.CurrentTexture.Width / 12;
        int frameHeight = player.CurrentTexture.Height;

        int xPosition = frameIndex * frameWidth;
        player.sourceRectangle = new Rectangle(xPosition, 0, frameWidth, frameHeight);
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
            {
                // this line is causing a double jump
                // TODO: fix double jump
                //player.ChangeState(new JumpState());

                // this will change later when we implement collision detection etc
                if (player.Position.Y >= 200)
                {
                    player.Position.Y = 200;
                    player.ChangeState(new IdleState());
                }
            }
            else
                player.ChangeState(new IdleState());
        }
    }

    public void Reset(Player player)
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
    public void Heal(Player player)
    {
        // Can't heal while moving/jumping/attacking/damaged - do nothing
    }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Draw(Player player) { }
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

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }
    public void Heal(Player player)
    {
        // Can't heal while moving/jumping/attacking/damaged - do nothing
    }


    public void ReturnToIdle(Player player)
    {
        player.ChangeState(new IdleState());
    }
    public void Walk(Player player, int direction) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) { }
    public void Draw(Player player) { }
}

public class HealingState : IPlayerState
{
    private float healTimer = 0f;
    private const float healDuration = 0.5f;
    private float blinkTimer = 0f;

    public void Reset(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        healTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Toggle between yellow and white for flashing
        if (blinkTimer >= 0.1f)
        {
            player.DrawColor = (player.DrawColor == Color.Yellow) ? Color.White : Color.Yellow;
            blinkTimer = 0f;
        }

        if (healTimer >= healDuration)
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
    public void Heal(Player player) { }
}
