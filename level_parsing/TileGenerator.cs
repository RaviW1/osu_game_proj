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
        private Dictionary<string, Texture2D> tileTextures;
        public List<TileInformation> generateTileInfo;
        private List<TileBlock> tileList = new List<TileBlock>();
        public List<TileBlock> TileList
        {
            get { return tileList; }
        }
        public TileGenerator(List<TileInformation> generateTileInfo)
        {
            this.generateTileInfo = generateTileInfo;
        }
        // Load necessary tile textures into scope
        // Really just a helper function to declutter load_content in the Game1 class
        public void LoadTileTextures(ContentManager Content)
        {
            tileTextures = new Dictionary<string, Texture2D>();
            tileTextures.Add("floating_platform", Content.Load<Texture2D>("floating_platform1"));
            tileTextures.Add("level1_background", Content.Load<Texture2D>("level1_background"));
            tileTextures.Add("level_tile", Content.Load<Texture2D>("level_tile"));
            tileTextures.Add("right_cave_wall", Content.Load<Texture2D>("Level1_background_right"));
            tileTextures.Add("top_cave_wall", Content.Load<Texture2D>("Level1BG_Top"));
            tileTextures.Add("ground_platform", Content.Load<Texture2D>("Level1Ground"));
            tileTextures.Add("left_rocks_wall", Content.Load<Texture2D>("background_left_layer1"));

        }
        public void createMapBlocks(List<TileInformation> generateTileInfo)
        {
            // Note about drawing order: foreach iterates from list[0] to list [n] in order
            foreach (TileInformation tile in generateTileInfo)
            {
                TileBlock mapblock = new TileBlock(tileTextures[tile.tileType], tile.destRectangle, tile.isCollideable);
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
