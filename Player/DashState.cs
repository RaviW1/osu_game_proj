using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DashState : IPlayerState
{
    private float _dashDirection;

    public void OnEnter(Player player)
    {
        if (player.IsAirborne)
            player.HasAirDash = false;

        player.DashTimer = Player.DashDuration;
        player.DashCooldown = Player.DashCooldownDuration;
        player.IsDashing = true;

        if (player.IsAirborne)
            // air: None = right, FlipHorizontally = left
            _dashDirection = (player.facing == SpriteEffects.None) ? 1f : -1f;
        else
            // ground: flipped — sprite sheet default may differ
            _dashDirection = (player.facing == SpriteEffects.None) ? -1f : 1f;

        player.Velocity.Y = 0f;
        player.Velocity.X = _dashDirection * Player.DashSpeed;

        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);

        player.SuppressLandingTransition = true;
    }

    public void Update(Player player, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        player.DashTimer -= dt;

        player.Position.X += _dashDirection * Player.DashSpeed * dt;
        player.Velocity.X = _dashDirection * Player.DashSpeed;
        player.Velocity.Y = 0f;

        if (player.DashTimer <= 0f)
            EndDash(player);
    }

    public void Draw(Player player)
    {
        player.DrawColor = Color.CornflowerBlue;
    }

    public void TakeDamage(Player player)
    {
        EndDash(player);
        player.ChangeState(new DamagedState());
    }

    public void Walk(Player player, int direction) { }
    public void StopWalking(Player player) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void Heal(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void Dash(Player player) { }

    private void EndDash(Player player)
    {
        player.IsDashing = false;
        player.Velocity.X = 0f;
        player.Velocity.Y = 0f;
        player.DrawColor = Color.White;
        player.SuppressLandingTransition = false;

        player.ChangeState(player.OnGround ? (IPlayerState)new IdleState() : new FallingState());
    }
}