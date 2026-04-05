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
            CollisionDirection dir = GetDirection(bounds, tile.bounds, overlap, velocity);

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

    private static CollisionDirection GetDirection(Rectangle mover, Rectangle tile, Rectangle overlap, Vector2 velocity)
    {
        if (overlap.Width > overlap.Height)
        {
            // Vertical — use position, not velocity
            return mover.Center.Y < tile.Center.Y
                ? CollisionDirection.Down
                : CollisionDirection.Up;
        }
        else if (overlap.Height > overlap.Width)
        {
            // Horizontal — use velocity since position is less reliable here
            return velocity.X >= 0
                ? CollisionDirection.Right
                : CollisionDirection.Left;
        }
        else
        {
            // Perfect corner — dominant velocity axis decides
            if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
                return mover.Center.X < tile.Center.X
                    ? CollisionDirection.Right
                    : CollisionDirection.Left;
            else
                return mover.Center.Y < tile.Center.Y
                    ? CollisionDirection.Down
                    : CollisionDirection.Up;
        }
    }
}