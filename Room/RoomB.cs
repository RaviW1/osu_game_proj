using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using osu_game_proj;

public class RoomB : RoomBase
{
    public RoomB()
    {
        Bounds = new Rectangle(0, 0, 6000, 6000);
    }

    public override void Load(ContentManager content, TileGenerator tileGen)
    {
        Tiles.AddRange(tileGen.TileList);
    }
}