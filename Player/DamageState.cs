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

        // Apply gravity
        player.Velocity.Y += 500f * delta;
        player.Position.Y += player.Velocity.Y * delta;

        // TODO: should not be handled here
        // TODO: use physics collider
        //if (player.Tiles != null)
        //{
        //    foreach (var tile in player.Tiles)
        //    {
        //        if (tile.isCollideable && player.GetBounds().Intersects(tile.bounds))
        //        {
        //            player.HandleOverlap(tile.bounds);
        //        }
        //    }
        //}
        if (player.OnGround)
        {
            player.IsAirborne = false;
            player.ChangeState(new IdleState());
            return;
        }

        // Toggle visibility using a bool so it can never get stuck transparent
        if (blinkTimer >= 0.1f)
        {
            isVisible = !isVisible;
            player.DrawColor = isVisible ? Color.Red : Color.Transparent;
            blinkTimer = 0f;
        }

        // Exit state after duration — always force white and visible
        if (damageTimer >= damageDuration)
        {
            player.SuppressLandingTransition = false;
            player.Velocity = Vector2.Zero;
            player.DrawColor = Color.White;
            damageTimer = 0f;
            player.ChangeState(new IdleState());
            return;
        }
    }

    public void Reset(Player player)
    {
        damageTimer = 0f;
        blinkTimer = 0f;
        isVisible = true;
        player.DrawColor = Color.Red;
        player.Velocity = Vector2.Zero;
        player.CurrentTexture = player.Textures["Walking"];
        player.sourceRectangle = new Rectangle(0, 0, player.CurrentTexture.Width / 8, player.CurrentTexture.Height);
        player.SuppressLandingTransition = true;
    }

    public void Heal(Player player) { }
    public void ReturnToIdle(Player player) { player.ChangeState(new IdleState()); }
    public void Walk(Player player, int direction) { }
    public void Jump(Player player) { }
    public void Attack(Player player) { }
    public void TakeDamage(Player player) { }
    public void Draw(Player player) { }
}
