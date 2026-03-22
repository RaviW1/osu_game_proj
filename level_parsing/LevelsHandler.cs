using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace osu_game_proj
{

    public class LevelsHandler
    {
        private LoadLevelFile level1FileLoader;
        private TileGenerator tileGenObj1;
        private LoadLevelFile level2FileLoader;
        private TileGenerator tileGenObj2;
        private List<TileGenerator> levelGenList;
        private int currentLevelNum;

        public TileGenerator currentTilesGen;
        // TODO: incorporate Geo logic and use this in Game1
        public void LoadLevelTiles(ContentManager Content)
        {
            // TODO: Create new method for these code blocks

            // Load level1
            List<TileInformation> generateTileInfo = new List<TileInformation>();
            level1FileLoader = new LoadLevelFile();
            level1FileLoader.LoadFile("level_files/test_level.xml", generateTileInfo);

            tileGenObj1 = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj1.LoadTileTextures(Content);

            levelGenList.Add(tileGenObj1);
            generateTileInfo.Clear();

            // Load Level 2
            level2FileLoader = new LoadLevelFile();
            level2FileLoader.LoadFile("level_files/test_level2.xml", generateTileInfo);

            tileGenObj2 = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj2.LoadTileTextures(Content);

            levelGenList.Add(tileGenObj2);
            generateTileInfo.Clear();

            currentTilesGen = tileGenObj1;
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
                    //instance.geos = instance.geosLevel1;

                }
                else
                {
                    currentLevelNum = 0;
                    currentTilesGen = levelGenList[currentLevelNum];
                }
            }
            else if (direction == 1)
            {
                currentLevelNum++;
                currentTilesGen = levelGenList[currentLevelNum];
                //instance.geos = instance.geosLevel2;
            }
        }
    }
}
