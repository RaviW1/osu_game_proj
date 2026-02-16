using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ItemManager
{
    private readonly List<IItem> items;
    private int currentIndex;

    public ItemManager()
    {
        items = new List<IItem>();
        currentIndex = 0;
    }

    public void AddItem(IItem item)
    {
        items.Add(item);
    }

    /// <summary>
    /// Cycle to the next or previous item in the list.
    /// </summary>
    /// <param name="direction">+1 for next item, -1 for previous item</param>
    public void CycleItem(int direction)
    {
        if (items.Count == 0) return;
        currentIndex = (currentIndex + direction + items.Count) % items.Count;
        items[currentIndex].Reset();
    }

    public void Update(GameTime gameTime)
    {
        if (items.Count > 0)
        {
            items[currentIndex].Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        if (items.Count > 0)
        {
            items[currentIndex].Draw(spriteBatch, position);
        }
    }
}
