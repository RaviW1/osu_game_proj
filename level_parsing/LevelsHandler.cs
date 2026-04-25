using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class LevelsHandler
    {
        private int currentLevelNum;
        public Texture2D geoTexture;
        private List<IRoom> allRoomObjs;
        private Dictionary<string, LevelNode> levelMap;

        public List<Geo> currentGeos;
        public TileGenerator currentTilesGen;
        public EnemyGenerator currentEnemyGen;
        public IRoom currentRoom;
        public LevelNode currentLevel;
        public int TotalRooms => levelMap.Count;

        public int CurrentRoomIndex
        {
            get
            {
                int i = 0;
                foreach (var key in levelMap.Keys)
                {
                    if (key == currentRoom.roomName) return i;
                    i++;
                }
                return 0;
            }
        }

        public (TileGenerator, EnemyGenerator, List<ExitInfo>, List<SpawnInfo>) LoadSingleLevel(string level_path, ContentManager Content)
        {
            List<EnemyInformation> generateEnemyInfo = new List<EnemyInformation>();
            List<TileInformation> generateTileInfo = new List<TileInformation>();
            List<ExitInfo> exits = new List<ExitInfo>();
            List<SpawnInfo> spawns = new List<SpawnInfo>();

            LoadLevelFile levelFileLoader = new LoadLevelFile();
            levelFileLoader.LoadFile(level_path, generateTileInfo, generateEnemyInfo, exits, spawns);

            TileGenerator tileGenObj = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj.LoadTileTextures(Content);
            EnemyGenerator enemyGenObj = new EnemyGenerator(new List<EnemyInformation>(generateEnemyInfo));
            enemyGenObj.LoadEnemyTextures(Content);
            return (tileGenObj, enemyGenObj, exits, spawns);
        }

        public void LoadLevelTiles(ContentManager Content)
        {
            geoTexture = Content.Load<Texture2D>("Geo - HUD_coin_shop");

            levelMap = new Dictionary<string, LevelNode>();
            LevelNode currentLevel = new LevelNode();
            LinkLevels(Content);
        }

        private void LinkLevels(ContentManager Content)
        {
            string json = File.ReadAllText("level_files/level_layout.json");
            var definitions = JsonSerializer.Deserialize<List<RoomDefinition>>(json);

            foreach (var def in definitions)
            {
                string name = def.Name;
                string filePath = def.File;

                var (tGen, eGen, exits, spawns) = this.LoadSingleLevel(filePath, Content);

                LevelNode node = new LevelNode();
                node.Name = def.Name;
                node.TileGen = tGen;
                node.EnemyGen = eGen;
                node.Exits = exits;
                node.Spawns = spawns;

                node.Room = (def.Type == "RoomA") ? new RoomA() : new RoomB();
                node.Room.Load(Content, node.TileGen);
                node.Room.roomName = def.Name;

                // Wire exits and spawns from XML into the room
                node.Room.SetExits(exits);
                node.Room.SetSpawns(spawns);

                // Generate Geos
                node.Geos = new List<Geo>();
                Geo.PlaceGeosOnPlatforms(node.TileGen, node.Geos, geoTexture);

                levelMap.Add(def.Name, node);
            }

            // Stitch the levels together
            foreach (var def in definitions)
            {
                LevelNode current = levelMap[def.Name];

                if (def.Left != null) current.Room.LeftNeighbor = levelMap[(string)def.Left].Room;
                if (def.Right != null) current.Room.RightNeighbor = levelMap[(string)def.Right].Room;
                if (def.Up != null) current.Room.UpNeighbor = levelMap[(string)def.Up].Room;
                if (def.Down != null) current.Room.DownNeighbor = levelMap[(string)def.Down].Room;
            }

            // Set Initial Level
            var firstLevel = levelMap["level1"];
            currentRoom = firstLevel.Room;
            currentTilesGen = firstLevel.TileGen;
            currentGeos = firstLevel.Geos;
            currentEnemyGen = firstLevel.EnemyGen;
            currentLevel = firstLevel;
            currentLevelNum = firstLevel.Room.roomIndex;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentTilesGen.Draw(spriteBatch);
        }

        public void DrawEnemies(SpriteBatch spriteBatch)
        {
            currentEnemyGen.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime, Player player, SpatialGrid _grid)
        {
            currentEnemyGen.Update(gameTime, player, _grid);
        }

        public void CycleStage(int direction)
        {
            IRoom nextRoom = null;

            if (direction == -1) nextRoom = currentRoom.LeftNeighbor;
            else if (direction == 1) nextRoom = currentRoom.RightNeighbor;
            else if (direction == 2) nextRoom = currentRoom.UpNeighbor;
            else if (direction == -2) nextRoom = currentRoom.DownNeighbor;

            if (nextRoom != null)
            {
                currentRoom = nextRoom;

                currentLevel = levelMap[currentRoom.roomName];
                currentTilesGen = currentLevel.TileGen;
                currentEnemyGen = currentLevel.EnemyGen;
                currentGeos = currentLevel.Geos;

                int newRoomNum = currentRoom.roomIndex;
                currentLevelNum = newRoomNum;
            }
        }

        public void ResetToFirstLevel()
        {
            var firstLevel = levelMap["level1"];
            currentRoom = firstLevel.Room;
            currentTilesGen = firstLevel.TileGen;
            currentEnemyGen = firstLevel.EnemyGen;
            currentGeos = firstLevel.Geos;
            currentLevel = firstLevel;
            currentLevelNum = firstLevel.Room.roomIndex;
        }

        public void ResetAllEnemies()
        {
            foreach (var node in levelMap.Values)
                node.EnemyGen.ResetEnemies();
        }

        public void ClearGeos()
        {
            foreach (var node in levelMap.Values)
            {
                node.Geos.Clear();
                Geo.PlaceGeosOnPlatforms(node.TileGen, node.Geos, geoTexture);
            }
            currentGeos = currentLevel.Geos;
        }
    }
}