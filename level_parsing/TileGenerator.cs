using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class TileGenerator
    {
        // TODO: might need to add order to drawing - think about later
        private Dictionary<int, Texture2D> tileTextures;
        private List<TileInformation> generateTileInfo;
        private List<TileBlock> tileList;
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
            tileTextures = new Dictionary<int, Texture2D>();
            tileTextures.Add(1, Content.Load<Texture2D>("floating_platform1"));
        }
        public void createMapBlocks(List<TileInformation> generateTileInfo)
        {
            // foreach iterates from list[0] to list [n] in order
            tileList = new List<TileBlock>();
            foreach (TileInformation tile in generateTileInfo)
            {
                TileBlock mapblock = new TileBlock(tileTextures[tile.tileType], tile.destRectangle);
                tileList.Add(mapblock);
            }

        }
        // TODO: Write class that iterates through list of mapBlock objects
        // TODO: and draws them to creen
        public void Draw(SpriteBatch spriteBatch)
        {
            this.createMapBlocks(this.generateTileInfo);
            foreach (TileBlock block in this.tileList)
            {
                block.Draw(spriteBatch);
            }
        }
    }
}
