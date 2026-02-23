using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextureItem : IItem
{
    private readonly Texture2D texture;
    private readonly Action<Player> selectAction;
    private readonly Action<Player> deselectAction;
    private float timer;

    public int Id { get; }

    public TextureItem(int id, Texture2D texture, Action<Player> selectAction = null, Action<Player> deselectAction = null)
    {
        Id = id;
        this.texture = texture;
        this.selectAction = selectAction;
        this.deselectAction = deselectAction;
        timer = 0f;
    }

    public void Update(GameTime gameTime)
    {
        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, bool isSelected)
    {
        float bob = (float)Math.Sin(timer * 2.5) * 5f;
        Vector2 drawPos = new Vector2(position.X, position.Y + bob);

        Color color = isSelected ? Color.White : Color.Gray * 0.5f;
        spriteBatch.Draw(texture, drawPos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public void Reset()
    {
        timer = 0f;
    }

    public void OnSelect(Player player)
    {
        selectAction?.Invoke(player);
    }

    public void OnDeselect(Player player)
    {
        deselectAction?.Invoke(player);
    }
}
