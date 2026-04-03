using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public static class CollisionSystem
{
    public static List<CollisionResult> Query(Rectangle bounds, List<TileBlock> tiles, Vector2 velocity)
    {
        var results = new List<CollisionResult>();

        foreach (var tile in tiles)
        {
            if (!tile.isCollideable && !tile.isHarmful) continue;
            if (!bounds.Intersects(tile.bounds)) continue;

            Rectangle overlap = Rectangle.Intersect(bounds, tile.bounds);
            CollisionDirection dir = GetDirection(overlap, velocity);

            results.Add(new CollisionResult
            {
                Tile = tile,
                Overlap = overlap,
                Direction = dir,
                IsHarmful = tile.isHarmful,
                IsCollideable = tile.isCollideable
            });
        }

        // Vertical collisions resolved before horizontal
        results.Sort((a, b) =>
        {
            bool aVert = a.Direction == CollisionDirection.Up || a.Direction == CollisionDirection.Down;
            bool bVert = b.Direction == CollisionDirection.Up || b.Direction == CollisionDirection.Down;
            if (aVert && !bVert) return -1;
            if (!aVert && bVert) return 1;
            return 0;
        });

        return results;
    }

    private static CollisionDirection GetDirection(Rectangle overlap, Vector2 velocity)
    {
        if (overlap.Width > overlap.Height)
        {
            return velocity.Y >= 0 ? CollisionDirection.Down : CollisionDirection.Up;
        }
        else if (overlap.Height > overlap.Width)
        {
            return velocity.X >= 0 ? CollisionDirection.Right : CollisionDirection.Left;
        }
        else
        {
            // Perfect corner — strictly use dominant velocity axis
            // No bias — if you didn't clear the height you don't land
            if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
                return velocity.X >= 0 ? CollisionDirection.Right : CollisionDirection.Left;
            else
                return velocity.Y >= 0 ? CollisionDirection.Down : CollisionDirection.Up;
        }
    }
}