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
        // we assign an arbitrary integer value to each tile
        public void LoadTileTextures(ContentManager Content)
        {
            // TODO: Change integers indexes to texture names 
            tileTextures = new Dictionary<int, Texture2D>();
            tileTextures.Add(1, Content.Load<Texture2D>("floating_platform1"));
            tileTextures.Add(2, Content.Load<Texture2D>("level1_background"));
            tileTextures.Add(3, Content.Load<Texture2D>("level_tile"));
            tileTextures.Add(4, Content.Load<Texture2D>("Level1_background_right"));
            tileTextures.Add(5, Content.Load<Texture2D>("Level1BG_Top"));
            tileTextures.Add(6, Content.Load<Texture2D>("Level1Ground"));
            tileTextures.Add(7, Content.Load<Texture2D>("background_left_layer1"));

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
