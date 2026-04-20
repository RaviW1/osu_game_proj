using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AttackUpState : IPlayerState
{
    private const float AttackDuration = 0.3f;
    private const float SecondsPerFrame = 0.1f;
    private const int TotalFrames = 6;
    private float attackTimer = 0f;
    private bool wasJumping = false;
    private int currentFrame = 0;
    private float timeSinceLastFrame = 0f;
    public void OnEnter(Player player) { }
    public void Update(Player player, GameTime gameTime) { }
    public void Draw(Player player, SpriteBatch spriteBatch) { }
    public void Walk(Player player, int direction) { }
    public void StopWalking(Player player) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) { }
    public void Heal(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void Dash(Player player) { }
    public void LookUp(Player player) { }
}
