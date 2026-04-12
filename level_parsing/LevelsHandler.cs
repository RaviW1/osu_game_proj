using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class LevelsHandler
    {
        private List<TileGenerator> levelGenList;
        private List<EnemyGenerator> enemyGenList;
        private int currentLevelNum;
        public Texture2D geoTexture;
        private List<List<Geo>> allLevelGeos;
        private List<IRoom> allRoomObjs;

        public List<Geo> currentGeos;
        public TileGenerator currentTilesGen;
        public EnemyGenerator currentEnemyGen;
        public IRoom currentRoom;

        public void LoadSingleLevel(string level_path, ContentManager Content)
        {

            List<EnemyInformation> generateEnemyInfo = new List<EnemyInformation>();
            List<TileInformation> generateTileInfo = new List<TileInformation>();
            LoadLevelFile levelFileLoader = new LoadLevelFile();
            levelFileLoader.LoadFile(level_path, generateTileInfo, generateEnemyInfo);

            TileGenerator tileGenObj = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj.LoadTileTextures(Content);
            EnemyGenerator enemyGenObj = new EnemyGenerator(new List<EnemyInformation>(generateEnemyInfo));
            enemyGenObj.LoadEnemyTextures(Content);

            levelGenList.Add(tileGenObj);
            enemyGenList.Add(enemyGenObj);
            generateTileInfo.Clear();
            generateEnemyInfo.Clear();
        }
        // TODO: add arg to specify starting level
        public void LoadLevelTiles(ContentManager Content)
        {
            // TODO: Create new method for these code blocks

            geoTexture = Content.Load<Texture2D>("Geo - HUD_coin_shop");
            levelGenList = new List<TileGenerator>();
            enemyGenList = new List<EnemyGenerator>();
            allLevelGeos = new List<List<Geo>>();
            allRoomObjs = new List<IRoom>();

            // To add a new level:
            // add xml to load here 
            // create a new Room object to handle collisions
            // Load level1
            this.LoadSingleLevel("level_files/test_level.xml", Content);
            RoomA roomA = new RoomA();
            roomA.Load(Content, levelGenList[0]);
            allRoomObjs.Add(roomA);

            // Load Level 2
            this.LoadSingleLevel("level_files/test_level2.xml", Content);
            RoomB roomB = new RoomB();
            roomB.Load(Content, levelGenList[1]);
            allRoomObjs.Add(roomB);

            foreach (TileGenerator tileGen in levelGenList)
            {
                List<Geo> geo_level = new List<Geo>();
                Geo.PlaceGeosOnPlatforms(tileGen, geo_level, geoTexture);
                allLevelGeos.Add(geo_level);
            }

            currentEnemyGen = enemyGenList[0];
            currentTilesGen = levelGenList[0];
            currentGeos = allLevelGeos[0];
            currentRoom = allRoomObjs[0];
            currentLevelNum = 0;

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
            if (direction == -1)
            {
                if (currentLevelNum != 0)
                {
                    currentLevelNum--;
                    currentTilesGen = levelGenList[currentLevelNum];
                    currentEnemyGen = enemyGenList[currentLevelNum];
                    currentGeos = allLevelGeos[currentLevelNum];
                    currentRoom = allRoomObjs[currentLevelNum];

                }
                else
                {
                    currentLevelNum = 0;
                    currentTilesGen = levelGenList[currentLevelNum];
                    currentEnemyGen = enemyGenList[currentLevelNum];
                    currentGeos = allLevelGeos[currentLevelNum];
                    currentRoom = allRoomObjs[currentLevelNum];
                }
            }
            else if (direction == 1)
            {
                if (currentLevelNum < levelGenList.Count - 1)
                {
                    currentLevelNum++;
                    currentTilesGen = levelGenList[currentLevelNum];
                    currentEnemyGen = enemyGenList[currentLevelNum];
                    currentGeos = allLevelGeos[currentLevelNum];
                    currentRoom = allRoomObjs[currentLevelNum];
                }
            }
        }
        public void ResetAllEnemies()
        {
            foreach (EnemyGenerator enemyGen in enemyGenList)
                enemyGen.ResetEnemies();
        }

        public void ClearGeos()
        {
            allLevelGeos.Clear();
            foreach (TileGenerator tileGen in levelGenList)
            {
                List<Geo> geo_level = new List<Geo>();
                Geo.PlaceGeosOnPlatforms(tileGen, geo_level, geoTexture);
                allLevelGeos.Add(geo_level);
            }
            currentGeos = allLevelGeos[currentLevelNum];
        }
    }
}
