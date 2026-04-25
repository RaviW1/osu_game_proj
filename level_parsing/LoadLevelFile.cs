using System.Xml.Linq;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class LoadLevelFile
    {
        public void LoadFile(string filepath, List<TileInformation> generateTileInfo, List<EnemyInformation> generateEnemyInfo)
        {
            LoadFile(filepath, generateTileInfo, generateEnemyInfo, null, null);
        }

        public void LoadFile(string filepath, List<TileInformation> generateTileInfo, List<EnemyInformation> generateEnemyInfo, List<ExitInfo> exits, List<SpawnInfo> spawns)
        {
            XDocument doc = XDocument.Load(filepath);
            string levelName = doc.Root.Attribute("name").Value;

            foreach (var t in doc.Descendants("Tile"))
            {
                TileInformation tileInfo = new TileInformation(
                    (string)t.Attribute("id"),
                    (int)t.Attribute("x_pos"),
                    (int)t.Attribute("y_pos"),
                    (int)t.Attribute("x_size"),
                    (int)t.Attribute("y_size"),
                    (int)t.Attribute("collide"),
                    (int)t.Attribute("damage"));
                generateTileInfo.Add(tileInfo);
            }

            foreach (var e in doc.Descendants("Enemy"))
            {
                EnemyInformation enemyInfo = new EnemyInformation(
                    (string)e.Attribute("id"),
                    (int)e.Attribute("x_pos"),
                    (int)e.Attribute("y_pos"));
                generateEnemyInfo.Add(enemyInfo);
            }

            if (exits != null)
            {
                foreach (var ex in doc.Descendants("Exit"))
                {
                    exits.Add(new ExitInfo(
                        (string)ex.Attribute("direction"),
                        (int)ex.Attribute("x_pos"),
                        (int)ex.Attribute("y_pos"),
                        (int)ex.Attribute("width"),
                        (int)ex.Attribute("height")));
                }
            }

            if (spawns != null)
            {
                foreach (var sp in doc.Descendants("Spawn"))
                {
                    spawns.Add(new SpawnInfo(
                        (string)sp.Attribute("id"),
                        (int)sp.Attribute("x_pos"),
                        (int)sp.Attribute("y_pos")));
                }
            }
        }
    }
}