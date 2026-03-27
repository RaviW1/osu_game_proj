using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Camera2D
{
    private readonly GraphicsDevice _graphics;

    public Vector2 Position { get; private set; }
    public float Zoom { get; set; } = 1f;
    public Rectangle RoomBounds { get; set; }

    private const float LerpSpeed = 0.08f;

    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
    }

    public void Follow(Vector2 target)
    {
        var vp = _graphics.Viewport;
        Vector2 desired = new Vector2(
            target.X - vp.Width / 2f,
            target.Y - vp.Height / 2f
        );
        Position = Vector2.Lerp(Position, desired, LerpSpeed);
        Position = new Vector2(
            Math.Clamp(Position.X, RoomBounds.Left, RoomBounds.Right - vp.Width),
            Math.Clamp(Position.Y, RoomBounds.Top, RoomBounds.Bottom - vp.Height)
        );
    }

    public void SnapTo(Vector2 target)
    {
        var vp = _graphics.Viewport;
        Position = new Vector2(
            target.X - vp.Width / 2f,
            target.Y - vp.Height / 2f
        );
        Position = new Vector2(
            Math.Clamp(Position.X, RoomBounds.Left, RoomBounds.Right - vp.Width),
            Math.Clamp(Position.Y, RoomBounds.Top, RoomBounds.Bottom - vp.Height)
        );
    }

    public Matrix GetTransform()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0)
             * Matrix.CreateScale(Zoom, Zoom, 1f);
    }
}