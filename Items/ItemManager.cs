using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ItemManager
{
    private readonly List<IItem> items;
    private readonly List<bool> equipped;
    private readonly float scale;

    public int Count => items.Count;
    public int VisibleCount { get; set; }
    public int MaxEquipped => VisibleCount > 0 ? VisibleCount - 1 : 0;

    public int EquippedCount
    {
        get
        {
            int c = 0;
            for (int i = 0; i < equipped.Count; i++)
                if (equipped[i]) c++;
            return c;
        }
    }

    public ItemManager(float scale = 0.4f)
    {
        items = new List<IItem>();
        equipped = new List<bool>();
        this.scale = scale;
        VisibleCount = 0;
    }

    public void AddItem(IItem item)
    {
        items.Add(item);
        equipped.Add(false);
    }

    public void EquipItem(int index, Player player)
    {
        if (index < 0 || index >= items.Count) return;
        if (equipped[index]) return;
        items[index].OnSelect(player);
        equipped[index] = true;
    }

    public void ToggleItem(int index, Player player)
    {
        if (index < 0 || index >= items.Count) return;

        if (equipped[index])
        {
            items[index].OnDeselect(player);
            equipped[index] = false;
        }
        else
        {
            if (EquippedCount >= MaxEquipped) return;
            items[index].OnSelect(player);
            equipped[index] = true;
        }
    }

    public bool IsEquipped(int index) => index >= 0 && index < equipped.Count && equipped[index];

    public IItem GetItem(int index) => items[index];

    public void Update(GameTime gameTime)
    {
        foreach (var item in items)
            item.Update(gameTime);
    }

    public void UnequipAll(Player player)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (equipped[i])
            {
                items[i].OnDeselect(player);
                equipped[i] = false;
            }
        }
    }
}
