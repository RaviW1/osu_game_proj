using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public interface IPlayerState
{
    void OnEnter(Player player);
    void Update(Player player, GameTime gameTime);
    void Draw(Player player, SpriteBatch spriteBatch);
    void Walk(Player player, int direction);
    void StopWalking(Player player);
    void Jump(Player player);
    void Attack(Player player);
    void TakeDamage(Player player);
    void Heal(Player player);
    void JumpHeld(Player player, float deltaTime);
    void Dash(Player player);
    void LookUp(Player player);
}
