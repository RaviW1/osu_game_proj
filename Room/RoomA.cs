using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using osu_game_proj;

public class RoomA : RoomBase
{
    public RoomA()
    {
        Bounds = new Rectangle(0, 0, 900, 900);
    }

    public override void Load(ContentManager content, TileGenerator tileGen)
    {
        Tiles.AddRange(tileGen.TileList);

        spawnPoints["default"] = new Vector2(350, 370);
        spawnPoints["fromRight"] = new Vector2(850, 370);
        spawnPoints["fromLeft"] = new Vector2(50, 370);
    }
}
