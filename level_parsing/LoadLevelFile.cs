using System.Xml.Linq;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class LoadLevelFile
    {
        public void LoadFile(string filepath, List<TileInformation> generateTileInfo, List<EnemyInformation> generateEnemyInfo)
        {
            XDocument doc = XDocument.Load(filepath);

            string levelName = doc.Root.Attribute("name").Value;

            foreach (var t in doc.Descendants("Tile"))
            {
                // Create new tile object for each tile in XML File
                //  for each tile object we add to a list then pass that list to tileGenerator
                TileInformation tileInfo = new TileInformation((string)t.Attribute("id"), (int)t.Attribute("x_pos"), (int)t.Attribute("y_pos"), (int)t.Attribute("x_size"), (int)t.Attribute("y_size"), (int)t.Attribute("collide"), (int)t.Attribute("damage"));
                generateTileInfo.Add(tileInfo);
            }
            foreach (var e in doc.Descendants("Enemy"))
            {
                // Create new object to hold info about each Enemy
                EnemyInformation enemyInfo = new EnemyInformation((string)e.Attribute("id"), (int)e.Attribute("x_pos"), (int)e.Attribute("y_pos"));
                generateEnemyInfo.Add(enemyInfo);
            }
        }
    }

}
