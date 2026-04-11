using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class HealingState : IPlayerState
{
    private float healTimer = 0f;
    private const float healDuration = 0.5f;
    private float blinkTimer = 0f;

    public void OnEnter(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        healTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Toggle between yellow and white for flashing
        if (blinkTimer >= 0.1f)
        {
            player.DrawColor = (player.DrawColor == Color.Yellow) ? Color.White : Color.Yellow;
            blinkTimer = 0f;
        }

        if (healTimer >= healDuration)
        {
            player.DrawColor = Color.White; // Reset color
            player.ChangeState(new IdleState());
        }
    }

    public void Draw(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Walk(Player player, int direction) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) { }

    public void StopWalking(Player player) { }

    public void Dash(Player player) { }
    public void JumpHeld(Player player, float deltaTime) { }
    public void Heal(Player player) { }
    public void LookUp(Player player) { }
}
