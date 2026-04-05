using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class IdleState : IPlayerState
{
    // -------------------------------------------------------------------------
    // IPlayerState implementation
    // -------------------------------------------------------------------------

    // IdleState.OnEnter
    public void OnEnter(Player player)
    {
        player.SuppressLandingTransition = false;
        player.Velocity.X = 0;
        player.DrawColor = Color.White;
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (!player.OnGround && player.Velocity.Y > 0)
        {
            player.IsAirborne = true;
            player.ChangeState(new FallingState());
        }
    }

    public void Walk(Player player, int direction)
    {
        if (direction == 0) return;
        player.ChangeState(new WalkingState(direction));
    }
    public void Jump(Player player)
    {
        player.ChangeState(new JumpState());
    }
    public void Attack(Player player) => player.ChangeState(new AttackState());
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());
    public void Heal(Player player) => player.ChangeState(new HealingState());

    // No-ops
    public void Draw(Player player) { }

    public void JumpHeld(Player player, float deltaTime) { }
    // WalkingState.StopWalking
    public void StopWalking(Player player)
    {
        player.Velocity.X = 0;
        player.ChangeState(new IdleState());
    }
}
