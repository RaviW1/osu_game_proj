using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;
using System.Collections.Generic;

public class RoomA : RoomBase
{
    public RoomA()
    {
        Bounds = new Rectangle(0, 0, 400, 900);
    }

    public override void Load(ContentManager content)
    {
        List<TileInformation> tileInfo = new List<TileInformation>();
        LoadLevelFile loader = new LoadLevelFile();
        loader.LoadFile("level_files/test_level.xml", tileInfo);

        TileGenerator gen = new TileGenerator(tileInfo);
        gen.LoadTileTextures(content);

        Tiles.AddRange(gen.TileList);

        spawnPoints["default"] = new Vector2(350, 370);
        spawnPoints["fromRight"] = new Vector2(3100, 370);
    }
}