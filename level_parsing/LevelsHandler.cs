using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace osu_game_proj
{

    public class LevelsHandler
    {
        private List<TileGenerator> levelGenList;
        private int currentLevelNum;
        private Texture2D geoTexture;
        private List<List<Geo>> allLevelGeos;
        private List<IRoom> allRoomObjs;

        public List<Geo> currentGeos;
        public TileGenerator currentTilesGen;
        public IRoom currentRoom;

        public void LoadSingleLevel(string level_path, ContentManager Content)
        {

            List<TileInformation> generateTileInfo = new List<TileInformation>();
            LoadLevelFile levelFileLoader = new LoadLevelFile();
            levelFileLoader.LoadFile(level_path, generateTileInfo);

            TileGenerator tileGenObj = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj.LoadTileTextures(Content);

            levelGenList.Add(tileGenObj);
            generateTileInfo.Clear();
        }
        // TODO: add arg to specify starting level
        public void LoadLevelTiles(ContentManager Content)
        {
            // TODO: Create new method for these code blocks

            geoTexture = Content.Load<Texture2D>("Geo - HUD_coin_shop");
            levelGenList = new List<TileGenerator>();
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

            currentTilesGen = levelGenList[0];
            currentGeos = allLevelGeos[0];
            currentRoom = allRoomObjs[0];
            currentLevelNum = 0;

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentTilesGen.Draw(spriteBatch);
        }
        public void CycleStage(int direction)
        {
            if (direction == -1)
            {
                if (currentLevelNum != 0)
                {
                    currentLevelNum--;
                    currentTilesGen = levelGenList[currentLevelNum];
                    currentGeos = allLevelGeos[currentLevelNum];
                    currentRoom = allRoomObjs[currentLevelNum];

                }
                else
                {
                    currentLevelNum = 0;
                    currentTilesGen = levelGenList[currentLevelNum];
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
                    currentGeos = allLevelGeos[currentLevelNum];
                    currentRoom = allRoomObjs[currentLevelNum];
                }
            }
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
