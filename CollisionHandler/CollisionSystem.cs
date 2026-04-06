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
        float absVX = Math.Abs(velocity.X);
        float absVY = Math.Abs(velocity.Y);

        // Weight each overlap dimension by the opposing velocity axis.
        // When moving fast horizontally, effectiveHeight grows large, biasing toward
        // a horizontal collision result even if the raw overlap is wider than tall.
        // This prevents corner clips during a dash from being misread as vertical hits.
        float effectiveWidth = overlap.Width * (absVY + 1f);
        float effectiveHeight = overlap.Height * (absVX + 1f);

        if (effectiveHeight < effectiveWidth)
        {
            // Vertical collision — top or bottom
            return mover.Center.Y < tile.Center.Y
                ? CollisionDirection.Down
                : CollisionDirection.Up;
        }
        else if (effectiveWidth < effectiveHeight)
        {
            // Horizontal collision — left or right
            return velocity.X >= 0
                ? CollisionDirection.Right
                : CollisionDirection.Left;
        }
        else
        {
            // Perfect tie — dominant velocity axis decides
            if (absVX > absVY)
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