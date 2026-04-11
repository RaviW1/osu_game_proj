using Microsoft.Xna.Framework;

public class EnemyInformation
{
    public string enemyType;
    public Vector2 destPos;
    public bool isCollideable;
    public bool isHarmful;

    public EnemyInformation(string ID, int x_pos, int y_pos)
    {
        enemyType = ID;
        destPos = new Vector2(x_pos, y_pos);
    }
}
