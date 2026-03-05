using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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