
using Microsoft.Xna.Framework;
// class of objects to hold basic information for each Enemy
// also processes information about each Enemy
public class EnemyInformation
{
    public string tileType;
    public Vector2 destRectangle;
    public bool isCollideable;
    public bool isHarmful;

    public EnemyInformation(string ID, int x_pos, int y_pos)
    {
        tileType = ID;
        destRectangle = new Vector2(x_pos, y_pos);
    }
}
