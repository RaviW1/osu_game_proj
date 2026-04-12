using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using osu_game_proj;

public class RoomB : RoomBase
{
    public RoomB()
    {
        Bounds = new Rectangle(0, 0, 900, 900);
    }

    public override void Load(ContentManager content, TileGenerator tilGen)
    {
        Tiles.AddRange(tilGen.TileList);

        spawnPoints["default"] = new Vector2(350, 370);
        spawnPoints["fromLeft"] = new Vector2(50, 370);
        spawnPoints["fromRight"] = new Vector2(Bounds.Width - 50, 370);
    }
}
