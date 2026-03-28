using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LookUpState : IPlayerState
{
    private int direction = 1;

    public LookUpState(int direction)
    {
        this.direction = direction;
    }
    void Reset(Player player)
    {
    }
    void Draw(Player player);
    void Walk(Player player, int direction);
    void Jump(Player player);
    void Attack(Player player);
    void TakeDamage(Player player);
    void Heal(Player player);
    void JumpHeld(Player player, float deltaTime) { }
    void LookUp(Player player, int direction);
}
