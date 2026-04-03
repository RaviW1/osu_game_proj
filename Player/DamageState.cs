using Microsoft.Xna.Framework;

public class DamagedState : IPlayerState
{
    private float damageTimer = 0f;
    private const float damageDuration = 0.5f;
    private float blinkTimer = 0f;
    private bool isVisible = true;

    public void Update(Player player, GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        damageTimer += delta;
        blinkTimer += delta;

        // Gravity handled by Player.Update
        // Blink effect
        if (blinkTimer >= 0.1f)
        {
            isVisible = !isVisible;
            player.DrawColor = isVisible ? Color.Red : Color.Transparent;
            blinkTimer = 0f;
        }

        if (damageTimer >= damageDuration)
        {
            player.SuppressLandingTransition = false;
            player.Velocity = Vector2.Zero;
            player.DrawColor = Color.White;
            damageTimer = 0f;
            player.ChangeState(new IdleState());
        }
    }

    public void OnEnter(Player player)
    {
        damageTimer = 0f;
        blinkTimer = 0f;
        isVisible = true;
        player.DrawColor = Color.Red;
        player.Velocity = Vector2.Zero;
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(
            0, 0,
            player.CurrentTexture.Width / 8,
            player.CurrentTexture.Height);
        player.SuppressLandingTransition = true;
    }

    public void Heal(Player player) { }
    public void ReturnToIdle(Player player) => player.ChangeState(new IdleState());
    public void Walk(Player player, int direction) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }

    public void StopWalking(Player player) { }
    public void TakeDamage(Player player) { }
    public void Draw(Player player) { }
}
