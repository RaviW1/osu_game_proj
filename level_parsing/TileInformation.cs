
using System.Numerics;
using System;
// class of objects to hold basic information for each tile
// also processes information about each tile
public class TileInformation
{
    private Vector2 position;
    private int tileType;
    public TileInformation(string ID, string x, string y)
    {
        int x_int = Int32.Parse(x);
        int y_int = Int32.Parse(y);
        position = new Vector2(x_int, y_int);
        tileType = Int32.Parse(ID);
    }
}
