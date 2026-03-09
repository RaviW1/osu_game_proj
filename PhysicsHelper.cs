using Microsoft.Xna.Framework;
using System.Collections.Generic;
using osu_game_proj;


public static class PhysicsHelper
{
    // Returns true if there is a solid tile directly below the player's feet
    // TODO: change so we don't iterate through all colliders for a second time 
    public static bool HasGroundBelow(Player player)
    {
        Rectangle feet = new Rectangle(
            player.GetBounds().X,
            player.GetBounds().Bottom,
            player.GetBounds().Width,
            4);

        foreach (Rectangle tile in Game1.GetCurrentLevelColliders())
            if (feet.Intersects(tile)) return true;

        return false;
    }

    // Returns true if player is falling and their feet are near the top of a tile
    public static bool IsLandingOnTile(Player player, Rectangle tile)
    {
        Rectangle playerBounds = player.GetBounds();
        bool isFalling = player.Velocity.Y > 0;
        bool feetNearTop = playerBounds.Bottom - tile.Top < 20;

        return isFalling && feetNearTop;
    }

    // Snaps player to the top of a tile and zeroes vertical velocity
    public static void LandOnTile(Player player, Rectangle tile)
    {
        Rectangle playerBounds = player.GetBounds();
        player.Position.Y = tile.Top - (playerBounds.Height / 2f);
        player.Velocity.Y = 0f;
        player.IsAirborne = false;
    }
    public static void CheckCollisions(Player player, TileGenerator tileGen)
    {
        Rectangle playerBound = player.GetBounds();

        var tiles = tileGen.TileList;

        foreach (TileBlock tile in tiles)
        {
            if (tile.isCollideable)
            {
                if (playerBound.Intersects(tile.bounds))
                {
                    // TODO: logic for resolving collisions
                    player.HandleOverlap(tile.bounds);
                }
            }
        }
    }
}
