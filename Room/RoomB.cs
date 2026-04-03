using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using osu_game_proj;
using System.Collections.Generic;

public class RoomB : RoomBase
{
    public RoomB()
    {
        Bounds = new Rectangle(0, 0, 800, 900);
    }

    public override void Load(ContentManager content)
    {
        List<TileInformation> tileInfo = new List<TileInformation>();
        LoadLevelFile loader = new LoadLevelFile();
        loader.LoadFile("level_files/test_level2.xml", tileInfo);

        TileGenerator gen = new TileGenerator(tileInfo);
        gen.LoadTileTextures(content);

        Tiles.AddRange(gen.TileList);

        spawnPoints["default"] = new Vector2(350, 370);
        spawnPoints["fromLeft"] = new Vector2(100, 370);
    }
}