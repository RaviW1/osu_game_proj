using System.Collections.Generic;
using osu_game_proj;

public class LevelNode
{
    public string Name { get; set; }

    public RoomBase Room { get; set; }

    public TileGenerator TileGen { get; set; }

    public EnemyGenerator EnemyGen { get; set; }

    public List<Geo> Geos { get; set; }

}
public class RoomDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string File { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }
    public string Up { get; set; }
    public string Down { get; set; }
}
