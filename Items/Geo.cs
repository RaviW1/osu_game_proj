using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;
using System.Collections.Generic;

public class Geo
{
    private Texture2D texture;
    private Vector2 position;
    public bool IsCollected { get; private set; }

    private const float DrawScale = 0.35f;
    private const int BoundsWidth = 10;
    private const int BoundsHeight = 10;

    public Geo(Texture2D texture, Vector2 position)
    {
        this.texture = texture;
        this.position = position;
        IsCollected = false;
    }

    public void Update(GameTime gameTime)
    {
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)position.X, (int)position.Y,
            BoundsWidth, BoundsHeight);
    }

    public void Collect()
    {
        IsCollected = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsCollected) return;

        spriteBatch.Draw(texture, position, null, Color.White,
            0f, Vector2.Zero, DrawScale, SpriteEffects.None, 0f);
    }

    public static void PlaceGeosOnPlatforms(List<TileBlock> tiles, List<Geo> targetGeos, Texture2D geoTexture)
    {
        foreach (var tile in tiles)
        {
            if (tile.tileType == "floating_platform")
            {
                Rectangle platform = tile.bounds;
                int count = 3;
                float spacing = platform.Width / (float)(count + 1);
                for (int i = 1; i <= count; i++)
                {
                    float geoX = platform.X + spacing * i - 5;
                    float geoY = platform.Y - 18;
                    targetGeos.Add(new Geo(geoTexture, new Vector2(geoX, geoY)));
                }
            }
        }
    }
    public static void PlaceGeosOnPlatforms(TileGenerator tileGen, List<Geo> targetGeos, Texture2D geoTexture)
    {
        foreach (var tileInfo in tileGen.generateTileInfo)
        {
            if (tileInfo.tileType == "floating_platform")
            {
                Rectangle platform = tileInfo.destRectangle;
                int count = 3;
                float spacing = platform.Width / (float)(count + 1);
                for (int i = 1; i <= count; i++)
                {
                    float geoX = platform.X + spacing * i - 5;
                    float geoY = platform.Y - 18;
                    targetGeos.Add(new Geo(geoTexture, new Vector2(geoX, geoY)));
                }
            }
        }
    }
}
