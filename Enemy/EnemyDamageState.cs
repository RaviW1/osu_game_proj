using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class EnemyDamageState : IEnemyState
{
    public void Reset(Player player)
    {
        player.DrawColor = Color.White;
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        // If nothing is below us, start falling
        if (!player.OnGround)
        {
            player.IsAirborne = true;
            player.ChangeState(new FallingState());
            return;
        }
    }

    public void Walk(Player player, int direction) => player.ChangeState(new WalkingState(direction));
    public void Jump(Player player) => player.ChangeState(new JumpState());
    public void Attack(Player player) => player.ChangeState(new AttackState());
    public void TakeDamage(Player player) => player.ChangeState(new DamagedState());
    public void Heal(Player player) => player.ChangeState(new HealingState());

    // No-ops
    public void Draw(Player player) { }
}
