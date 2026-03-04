
using System.Numerics;
using System;
// class of objects to hold basic information for each tile
// also processes information about each tile
public class TileInformation
{
    public Vector2 position;
    public int tileType;
    public TileInformation(int ID, int x, int y)
    {
        int x_int = x;
        int y_int = y;
        position = new Vector2(x_int, y_int);
        tileType = ID;
    }
}
