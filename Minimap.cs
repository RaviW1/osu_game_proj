using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public static class Minimap
{
    private const int MapWidth   = 160;
    private const int MapHeight  = 90;
    private const int Margin     = 12;
    private const int BorderSize = 2;

    private const int RoomIndicatorSize    = 12;
    private const int RoomIndicatorSpacing = 6;

    public static void Draw(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        GraphicsDevice graphics,
        Rectangle roomBounds,
        List<TileBlock> tiles,
        Vector2 playerPos,
        int totalRooms,
        int currentRoomIndex)
    {
        int vpW = graphics.Viewport.Width;
        int vpH = graphics.Viewport.Height;

        int originX = vpW - MapWidth  - Margin;
        int originY = vpH - MapHeight - Margin;

        spriteBatch.Draw(pixel,
            new Rectangle(originX - BorderSize, originY - BorderSize,
                          MapWidth + BorderSize * 2, MapHeight + BorderSize * 2),
            Color.White * 0.6f);

        spriteBatch.Draw(pixel,
            new Rectangle(originX, originY, MapWidth, MapHeight),
            Color.Black * 0.75f);

        float scaleX = (float)MapWidth  / roomBounds.Width;
        float scaleY = (float)MapHeight / roomBounds.Height;

        Rectangle WorldToMap(Rectangle world)
        {
            int x = originX + (int)((world.X - roomBounds.X) * scaleX);
            int y = originY + (int)((world.Y - roomBounds.Y) * scaleY);
            int w = System.Math.Max(1, (int)(world.Width  * scaleX));
            int h = System.Math.Max(1, (int)(world.Height * scaleY));
            return new Rectangle(x, y, w, h);
        }

        foreach (var tile in tiles)
        {
            if (!tile.isCollideable) continue;
            spriteBatch.Draw(pixel, WorldToMap(tile.bounds), Color.Gray * 0.9f);
        }

        int px = originX + (int)((playerPos.X - roomBounds.X) * scaleX) - 2;
        int py = originY + (int)((playerPos.Y - roomBounds.Y) * scaleY) - 2;
        spriteBatch.Draw(pixel, new Rectangle(px, py, 4, 4), Color.Cyan);

        int totalWidth = totalRooms * RoomIndicatorSize + (totalRooms - 1) * RoomIndicatorSpacing;
        int indOriginX = Margin;
        int indOriginY = vpH - RoomIndicatorSize - Margin;

        for (int i = 0; i < totalRooms; i++)
        {
            int bx = indOriginX + i * (RoomIndicatorSize + RoomIndicatorSpacing);
            Color color = (i == currentRoomIndex) ? Color.Red : Color.Gray * 0.6f;

            spriteBatch.Draw(pixel,
                new Rectangle(bx - BorderSize, indOriginY - BorderSize,
                              RoomIndicatorSize + BorderSize * 2, RoomIndicatorSize + BorderSize * 2),
                Color.White * 0.4f);

            spriteBatch.Draw(pixel,
                new Rectangle(bx, indOriginY, RoomIndicatorSize, RoomIndicatorSize),
                color);
        }
    }
}