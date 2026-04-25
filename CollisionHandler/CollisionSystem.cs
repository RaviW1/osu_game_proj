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
        SortResults(results);
        return results;
    }

    public static List<CollisionResult> Query(Rectangle bounds, SpatialGrid grid, Vector2 velocity)
    {
        return Query(bounds, grid.GetNearby(bounds), velocity);
    }

    private static void SortResults(List<CollisionResult> results)
    {
        // Resolve vertical before horizontal
        results.Sort((a, b) =>
        {
            bool aVert = a.Direction == CollisionDirection.Up || a.Direction == CollisionDirection.Down;
            bool bVert = b.Direction == CollisionDirection.Up || b.Direction == CollisionDirection.Down;
            if (aVert && !bVert) return -1;
            if (!aVert && bVert) return 1;
            return 0;
        });
    }
    private static CollisionDirection GetDirection(Rectangle mover, Rectangle tile, Rectangle overlap, Vector2 velocity)
    {
        // Use raw overlap to determine the shallow axis
        // The smaller overlap dimension is the one we need to resolve
        if (overlap.Width < overlap.Height)
        {
            // Horizontal collision
            return mover.Center.X < tile.Center.X
                ? CollisionDirection.Right
                : CollisionDirection.Left;
        }
        else if (overlap.Height < overlap.Width)
        {
            // Vertical collision
            return mover.Center.Y < tile.Center.Y
                ? CollisionDirection.Down
                : CollisionDirection.Up;
        }
        else
        {
            // Perfect tie — use velocity to break it
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