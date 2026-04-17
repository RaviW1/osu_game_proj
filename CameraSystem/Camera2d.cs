using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Camera2D
{
    private readonly GraphicsDevice _graphics;
    private readonly Random _random = new Random();

    // Shake state
    private float _shakeIntensity;
    private int _shakeDuration;
    private const float ShakeDecay = 0.85f;

    public Vector2 Position { get; private set; }
    public float Zoom { get; set; } = 1f;
    public Rectangle RoomBounds { get; set; }

    private const float LerpSpeed = 0.08f;

    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
    }

    /// <summary>
    /// Trigger a screen shake.  Safe to call during hitstop — shake updates
    /// independently of gameplay so the freeze + shake combo feels correct.
    /// </summary>
    /// <param name="intensity">Max pixel offset per frame (e.g. 3f for mild, 6f for damage).</param>
    /// <param name="frames">How many frames the shake lasts before fully decaying.</param>
    public void Shake(float intensity, int frames)
    {
        // Take the stronger shake if one is already running
        if (intensity > _shakeIntensity)
            _shakeIntensity = intensity;
        if (frames > _shakeDuration)
            _shakeDuration = frames;
    }

    public void Follow(Vector2 target)
    {
        var vp = _graphics.Viewport;
        Vector2 desired = new Vector2(
            target.X - vp.Width / 2f,
            target.Y - vp.Height / 2f
        );
        Position = Vector2.Lerp(Position, desired, LerpSpeed);
        Position = Clamp(Position, vp);
    }

    public void SnapTo(Vector2 target)
    {
        var vp = _graphics.Viewport;
        Position = new Vector2(
            target.X - vp.Width / 2f,
            target.Y - vp.Height / 2f
        );
        Position = Clamp(Position, vp);
    }

    public Matrix GetTransform()
    {
        Vector2 shakeOffset = Vector2.Zero;

        if (_shakeDuration > 0)
        {
            shakeOffset = new Vector2(
                (float)(_random.NextDouble() * 2.0 - 1.0) * _shakeIntensity,
                (float)(_random.NextDouble() * 2.0 - 1.0) * _shakeIntensity
            );

            _shakeIntensity *= ShakeDecay;
            _shakeDuration--;

            // Kill tiny residual shake to avoid perpetual micro-jitter
            if (_shakeIntensity < 0.25f)
            {
                _shakeIntensity = 0f;
                _shakeDuration = 0;
            }
        }

        return Matrix.CreateTranslation(-(Position.X + shakeOffset.X), -(Position.Y + shakeOffset.Y), 0)
             * Matrix.CreateScale(Zoom, Zoom, 1f);
    }

    // ------------------------------------------------------------------ helpers

    private Vector2 Clamp(Vector2 pos, Viewport vp)
    {
        return new Vector2(
            Math.Clamp(pos.X, RoomBounds.Left, Math.Max(RoomBounds.Left, RoomBounds.Right - vp.Width)),
            Math.Clamp(pos.Y, RoomBounds.Top, Math.Max(RoomBounds.Top, RoomBounds.Bottom - vp.Height))
        );
    }
}