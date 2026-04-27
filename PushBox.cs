using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class PushBox
{
    private Texture2D _texture;
    public Vector2 Position;
    public Vector2 Velocity;
    public const int Size = 40;
    public Vector2 LastPosition;

    public PushBox(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        Position = position;
        Velocity = Vector2.Zero;
    }

    public Rectangle GetBounds() =>
        new Rectangle((int)Position.X, (int)Position.Y, Size, Size);

    public void Update(GameTime gameTime)
    {
        LastPosition = Position;
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity.Y += 500f * dt;
        Position += Velocity * dt;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, GetBounds(), Color.SaddleBrown);
    }
}