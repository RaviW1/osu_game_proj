using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DamagedState : IPlayerState
{
    private float damageTimer = 0f;
    private const float damageDuration = 0.5f;
    private float blinkTimer = 0f;

    public void Update(Player player, GameTime gameTime)
    {
        damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Apply gravity
        player.Velocity.Y += 20f;
        player.Position += player.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check floor
        if (player.Position.Y >= 370)
        {
            player.Position.Y = 370;
            player.Velocity.Y = 0f;
        }

        // Toggle between red and transparent for flashing
        if (blinkTimer >= 0.1f)
        {
            player.DrawColor = (player.DrawColor == Color.Red) ? Color.Transparent : Color.Red;
            blinkTimer = 0f;
        }

        if (damageTimer >= damageDuration)
        {
            player.DrawColor = Color.White;
            player.ChangeState(new IdleState());
        }
    }

    public void Reset(Player player)
    {
        player.Velocity.Y = 0f;
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
