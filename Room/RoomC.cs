using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using osu_game_proj;

public class RoomC : RoomBase
{
    public RoomC()
    {
        Bounds = new Rectangle(0, 0, 800, 900);
    }

    public override void Load(ContentManager content, TileGenerator tileGen)
    {
        Tiles.AddRange(tileGen.TileList);
    }
}
