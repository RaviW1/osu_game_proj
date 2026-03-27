using Microsoft.Xna.Framework;

public interface IEnemyState
{
    void Update(Player player, GameTime gameTime);
    void Reset(Player player);
    void Draw(Player player);
    void Walk(Player player, int direction);
    void Jump(Player player);
    void Attack(Player player);
    void TakeDamage(Player player);
    void Heal(Player player);
    void JumpHeld(Player player, float deltaTime) { }
}
