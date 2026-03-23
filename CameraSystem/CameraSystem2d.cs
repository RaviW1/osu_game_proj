dusing System;

public class Class1
{
	public Class1()
	{
	}
}
// Camera2D.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera2D
{
    private readonly GraphicsDevice _graphics;

    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1f;

    // How tightly the camera follows — 1.0 = instant snap, 0.1 = very floaty
    private const float LerpSpeed = 0.08f;

    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
    }

    public void Follow(Vector2 target)
    {
        var viewport = _graphics.Viewport;

        // Where we WANT the camera to be (target centered on screen)
        Vector2 desiredPosition = new Vector2(
            target.X - viewport.Width / 2f,
            target.Y - viewport.Height / 2f
        );

        // Smoothly lerp toward it
        Position = Vector2.Lerp(Position, desiredPosition, LerpSpeed);
    }

    public Matrix GetTransform()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0)
             * Matrix.CreateScale(Zoom, Zoom, 1f);
    }
}