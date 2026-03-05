using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ItemManager
{
    private readonly List<IItem> items;
    private readonly List<Vector2> positions;
    private int currentIndex;
    private readonly float scale;

    public ItemManager(float scale = 0.4f)
    {
        items = new List<IItem>();
        positions = new List<Vector2>();
        currentIndex = 0;
        this.scale = scale;
    }

    public void AddItem(IItem item, Vector2 position)
    {
        items.Add(item);
        positions.Add(position);
    }

    /// <param name="direction">+1 for next item, -1 for previous item</param>
    public void CycleItem(int direction, Player player)
    {
        if (items.Count == 0) return;
        items[currentIndex].OnDeselect(player);
        currentIndex = (currentIndex + direction + items.Count) % items.Count;
        items[currentIndex].Reset();
        items[currentIndex].OnSelect(player);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var item in items)
        {
            item.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Draw(spriteBatch, positions[i], scale, i == currentIndex);
        }
    }
}
