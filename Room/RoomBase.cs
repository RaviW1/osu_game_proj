using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using osu_game_proj;
using System.Collections.Generic;

public abstract class RoomBase : IRoom
{
    public Rectangle Bounds { get; protected set; }
    public List<TileBlock> Tiles { get; protected set; } = new List<TileBlock>();
    protected Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>();

    // Custom exit zones loaded from XML
    private List<ExitInfo> exitZones = new List<ExitInfo>();

    // Directional neighbors
    public RoomBase LeftNeighbor { get; set; }
    public RoomBase RightNeighbor { get; set; }
    public RoomBase UpNeighbor { get; set; }
    public RoomBase DownNeighbor { get; set; }

    public int roomIndex { get; set; }
    public string roomName { get; set; }

    public virtual void Load(ContentManager content, TileGenerator tileGen) { }

    public void SetExits(List<ExitInfo> exits)
    {
        exitZones = exits;
    }

    public void SetSpawns(List<SpawnInfo> spawns)
    {
        spawnPoints.Clear();
        foreach (var spawn in spawns)
        {
            spawnPoints[spawn.Id] = spawn.Position;
        }
    }

    public virtual void Update(GameTime gameTime, Player player, GameScene scene)
    {
        Rectangle playerBounds = player.GetBounds();

        foreach (var exit in exitZones)
        {
            if (!playerBounds.Intersects(exit.Bounds)) continue;

            switch (exit.Direction)
            {
                case "left":
                    if (LeftNeighbor != null) scene.CycleStage(-1);
                    break;
                case "right":
                    if (RightNeighbor != null) scene.CycleStage(1);
                    break;
                case "up":
                    if (UpNeighbor != null) scene.CycleStage(2);
                    break;
                case "down":
                    if (DownNeighbor != null) scene.CycleStage(-2);
                    break;
            }
            break; // Only trigger one exit per frame
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
        if (spawnPoints.TryGetValue("default", out Vector2 def))
            return def;
        return Vector2.Zero;
    }
}