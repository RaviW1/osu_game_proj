using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using osu_game_proj;
using System.Collections.Generic;

public class RoomB : RoomBase
{
    public RoomB()
    {
        Bounds = new Rectangle(0, 0, 1000, 900);
    }

    public override void Load(ContentManager content, TileGenerator tilGen)
    {
        Tiles.AddRange(tilGen.TileList);

        spawnPoints["default"] = new Vector2(350, 370);
        spawnPoints["fromLeft"] = new Vector2(100, 370);
    }
}
