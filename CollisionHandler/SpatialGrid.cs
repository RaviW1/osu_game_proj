using System.Collections.Generic;
using Microsoft.Xna.Framework;
public class SpatialGrid
{
    private readonly int _cellSize;
    private readonly Dictionary<(int, int), List<TileBlock>> _cells = new();

    public SpatialGrid(int cellSize, List<TileBlock> tiles)
    {
        _cellSize = cellSize;
        foreach (var tile in tiles)
            Register(tile);
    }

    private void Register(TileBlock tile)
    {
        int x0 = tile.bounds.Left / _cellSize;
        int x1 = tile.bounds.Right / _cellSize;
        int y0 = tile.bounds.Top / _cellSize;
        int y1 = tile.bounds.Bottom / _cellSize;

        for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            {
                var key = (x, y);
                if (!_cells.ContainsKey(key))
                    _cells[key] = new List<TileBlock>();
                _cells[key].Add(tile);
            }
    }

    public List<TileBlock> GetNearby(Rectangle bounds)
    {
        int x0 = bounds.Left / _cellSize;
        int x1 = bounds.Right / _cellSize;
        int y0 = bounds.Top / _cellSize;
        int y1 = bounds.Bottom / _cellSize;

        var seen = new HashSet<TileBlock>();
        var result = new List<TileBlock>();

        for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
            {
                if (_cells.TryGetValue((x, y), out var cell))
                    foreach (var tile in cell)
                        if (seen.Add(tile))
                            result.Add(tile);
            }
        return result;
    }
}