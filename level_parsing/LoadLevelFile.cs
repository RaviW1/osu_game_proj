using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class LoadLevelFile
    {
        public void LoadFile(string filepath, TileGenerator tileGenObj)
        {
            XDocument doc = XDocument.Load(filepath);

            string levelName = doc.Root.Attribute("name").Value;

            foreach (var t in doc.Descendants("Tile"))
            {
                // TODO: Create new tile object for each tile in XML File
                //  for each tile object we add to a list then pass that list to tileGenerator


            }
        }
    }

    // TODO: break this out into its own file
    public class TileGenerator
    {
        private Dictionary<int, Texture2D> tileTextures;
        private List<TileInformation> generateTileInfo;
        public TileGenerator(List<TileInformation> generateTileInfo)
        {
            this.generateTileInfo = generateTileInfo;
        }
        // Load necessary tile textures into scope
        // Really just a helper function to declutter load_content in the Game1 class
        public void LoadTileTextures(ContentManager Content)
        {
            // Load all tile textures here
            // we assign an arbitrary integer value to each tile
            tileTextures.Add(1, Content.Load<Texture2D>("floating_platform1"));
        }
        public void createMapBlocks(List<TileInformation> generateTileInfo)
        {

        }

        // TODO: Write class that iterates through list of mapBlock objects
        // TODO: and draws them to creen
        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
