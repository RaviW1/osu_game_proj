using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HealingState : IPlayerState
{
    private const float HealDuration = 1f;
    private const int TotalSoulCost = 30;
    private const float DrainRate = TotalSoulCost / HealDuration; // soul per second

    private float healTimer = 0f;
    private float blinkTimer = 0f;
    private float soulDrainAccumulator = 0f;
    private int soulDrained = 0;
    private bool finished = false;

    public void OnEnter(Player player)
    {
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
    }

    public void Update(Player player, GameTime gameTime)
    {
        if (finished) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        healTimer += dt;
        blinkTimer += dt;

        if (blinkTimer >= 0.1f)
        {
            player.DrawColor = (player.DrawColor == Color.Yellow) ? Color.White : Color.Yellow;
            blinkTimer = 0f;
        }

        // Continuous soul drain that lines up with integer Soul ticks
        soulDrainAccumulator += DrainRate * dt;
        int target = (int)Math.Min(TotalSoulCost, soulDrainAccumulator);
        int delta = target - soulDrained;
        if (delta > 0)
        {
            player.Soul = Math.Max(0, player.Soul - delta);
            soulDrained = target;
        }

        if (healTimer >= HealDuration)
        {
            // Make sure the full 30 is consumed regardless of float rounding
            int remaining = TotalSoulCost - soulDrained;
            if (remaining > 0)
            {
                player.Soul = Math.Max(0, player.Soul - remaining);
                soulDrained = TotalSoulCost;
            }

            player.PlayerHealth++;
            player.DrawColor = Color.White;
            finished = true;
            player.ChangeState(new IdleState());
        }
    }

    // Called from Player.InterruptHeal when the heal key is released.
    // Already-drained soul is intentionally NOT refunded.
    public void Interrupt(Player player)
    {
        if (finished) return;
        finished = true;
        player.DrawColor = Color.White;
        player.ChangeState(new IdleState());
    }

    public void Draw(Player player, SpriteBatch spriteBatch)
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
