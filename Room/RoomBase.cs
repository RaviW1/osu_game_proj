using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using osu_game_proj;
using System.Collections.Generic;

public abstract class RoomBase : IRoom
{
    public Rectangle Bounds { get; protected set; }
    public Rectangle LeftExit => new Rectangle(Bounds.X, Bounds.Y, 10, Bounds.Height);
    public Rectangle RightExit => new Rectangle(Bounds.Right - 10, Bounds.Y, 10, Bounds.Height);
    public List<TileBlock> Tiles { get; protected set; } = new List<TileBlock>();
    protected Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>();

    // Directional neighbors
    public RoomBase LeftNeighbor { get; set; }
    public RoomBase RightNeighbor { get; set; }
    public RoomBase UpNeighbor { get; set; }
    public RoomBase DownNeighbor { get; set; }
    public int roomIndex { get; set; }
    public string roomName { get; set; }

    public virtual void Load(ContentManager content, TileGenerator tilege) { }

    public virtual void Update(GameTime gameTime, Player player, GameScene scene)
    {
        Rectangle playerBounds = player.GetBounds();
        if (playerBounds.Intersects(LeftExit))
        {
            if (LeftNeighbor != null)
            {
                scene.CycleStage(-1);
            }
        }
        else if (playerBounds.Intersects(RightExit))
        {
            if (RightNeighbor != null)
            {
                scene.CycleStage(1);
            }
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.Draw(spriteBatch);
    }

    public virtual void Unload()
    {
        Tiles.Clear();
    }

    public Vector2 GetSpawnPoint(string entryId)
    {
        if (spawnPoints.TryGetValue(entryId, out Vector2 point))
            return point;
        return Vector2.Zero;
    }
}
