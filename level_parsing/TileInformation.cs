using Microsoft.Xna.Framework;
using System;
// class of objects to hold basic information for each tile
// also processes information about each tile
public class TileInformation
{
    public string tileType;
    public Rectangle destRectangle;
    public bool isCollideable;

    public TileInformation(string ID, int x_pos, int y_pos, int x_size, int y_size, int collide)
    {
        tileType = ID;
        destRectangle = new Rectangle(x_pos, y_pos, x_size, y_size);
        if (collide == 0)
        {
            isCollideable = false;
        }
        else if (collide == 1)
        {
            isCollideable = true;
        }
    }
}
