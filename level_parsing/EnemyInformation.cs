using Microsoft.Xna.Framework;

// class of objects to hold basic information for each tile
// also processes information about each tile
public class EnemyInformation
{
    public string enemyType;
    public System.Numerics.Vector2 destPos;
    public bool isCollideable;
    public bool isHarmful;

    public EnemyInformation(string ID, int x_pos, int y_pos)
    {
        enemyType = ID;
        destPos = new System.Numerics.Vector2(x_pos, y_pos);

    }
}
