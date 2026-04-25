using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class ExitInfo
    {
        public string Direction { get; set; } // "left", "right", "up", "down"
        public Rectangle Bounds { get; set; }

        public ExitInfo(string direction, int x, int y, int width, int height)
        {
            Direction = direction;
            Bounds = new Rectangle(x, y, width, height);
        }
    }

    public class SpawnInfo
    {
        public string Id { get; set; } // "default", "fromLeft", "fromRight", "fromUp", "fromDown"
        public Vector2 Position { get; set; }

        public SpawnInfo(string id, int x, int y)
        {
            Id = id;
            Position = new Vector2(x, y);
        }
    }
}