using Microsoft.Xna.Framework;

public struct CollisionResult
{
    public TileBlock Tile;
    public Rectangle Overlap;
    public CollisionDirection Direction;
    public bool IsHarmful;
    public bool IsCollideable;
}