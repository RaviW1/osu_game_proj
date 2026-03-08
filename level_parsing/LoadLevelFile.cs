using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class LoadLevelFile
    {
        public void LoadFile(string filepath, List<TileInformation> generateTileInfo)
        {
            XDocument doc = XDocument.Load(filepath);

            string levelName = doc.Root.Attribute("name").Value;

            foreach (var t in doc.Descendants("Tile"))
            {
                // TODO: Create new tile object for each tile in XML File
                //  for each tile object we add to a list then pass that list to tileGenerator
                TileInformation tileInfo = new TileInformation((string)t.Attribute("id"), (int)t.Attribute("x_pos"), (int)t.Attribute("y_pos"), (int)t.Attribute("x_size"), (int)t.Attribute("y_size"), (int)t.Attribute("collide"));
                generateTileInfo.Add(tileInfo);
            }
        }
    }

}
