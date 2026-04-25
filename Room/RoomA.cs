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
    }
}