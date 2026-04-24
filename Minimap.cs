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
 
    private const float BorderOpacity          = 0.6f;
    private const float BackgroundOpacity      = 0.75f;
    private const float TileOpacity            = 0.9f;
    private const float IndicatorBorderOpacity = 0.4f;
    private const float InactiveRoomOpacity    = 0.6f;
 
    private static readonly Color BorderColor        = Color.White;
    private static readonly Color BackgroundColor    = Color.Black;
    private static readonly Color TileColor          = Color.Gray;
    private static readonly Color PlayerColor        = Color.Cyan;
    private static readonly Color ActiveRoomColor    = Color.Red;
    private static readonly Color InactiveRoomColor  = Color.Gray;
    private static readonly Color IndicatorBorderColor = Color.White;
 
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
 
        float scaleX = (float)MapWidth  / roomBounds.Width;
        float scaleY = (float)MapHeight / roomBounds.Height;
 
        DrawBackground(spriteBatch, pixel, originX, originY);
        DrawTiles(spriteBatch, pixel, tiles, roomBounds, originX, originY, scaleX, scaleY);
        DrawPlayer(spriteBatch, pixel, playerPos, roomBounds, originX, originY, scaleX, scaleY);
        DrawRoomIndicators(spriteBatch, pixel, vpH, totalRooms, currentRoomIndex);
    }
 
    private static void DrawBackground(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        int originX,
        int originY)
    {
        spriteBatch.Draw(pixel,
            new Rectangle(originX - BorderSize, originY - BorderSize,
                          MapWidth + BorderSize * 2, MapHeight + BorderSize * 2),
            BorderColor * BorderOpacity);
 
        spriteBatch.Draw(pixel,
            new Rectangle(originX, originY, MapWidth, MapHeight),
            BackgroundColor * BackgroundOpacity);
    }
 
    private static void DrawTiles(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        List<TileBlock> tiles,
        Rectangle roomBounds,
        int originX,
        int originY,
        float scaleX,
        float scaleY)
    {
        foreach (var tile in tiles)
        {
            if (!tile.isCollideable) continue;
            spriteBatch.Draw(pixel,
                WorldToMap(tile.bounds, roomBounds, originX, originY, scaleX, scaleY),
                TileColor * TileOpacity);
        }
    }
 
    private static void DrawPlayer(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        Vector2 playerPos,
        Rectangle roomBounds,
        int originX,
        int originY,
        float scaleX,
        float scaleY)
    {
        int px = originX + (int)((playerPos.X - roomBounds.X) * scaleX) - 2;
        int py = originY + (int)((playerPos.Y - roomBounds.Y) * scaleY) - 2;
        spriteBatch.Draw(pixel, new Rectangle(px, py, 4, 4), PlayerColor);
    }
 
    private static void DrawRoomIndicators(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        int vpH,
        int totalRooms,
        int currentRoomIndex)
    {
        int indOriginX = Margin;
        int indOriginY = vpH - RoomIndicatorSize - Margin;
 
        for (int i = 0; i < totalRooms; i++)
        {
            int bx    = indOriginX + i * (RoomIndicatorSize + RoomIndicatorSpacing);
            Color color = (i == currentRoomIndex) ? ActiveRoomColor : InactiveRoomColor * InactiveRoomOpacity;
 
            spriteBatch.Draw(pixel,
                new Rectangle(bx - BorderSize, indOriginY - BorderSize,
                              RoomIndicatorSize + BorderSize * 2, RoomIndicatorSize + BorderSize * 2),
                IndicatorBorderColor * IndicatorBorderOpacity);
 
            spriteBatch.Draw(pixel,
                new Rectangle(bx, indOriginY, RoomIndicatorSize, RoomIndicatorSize),
                color);
        }
    }
 
    private static Rectangle WorldToMap(
        Rectangle world,
        Rectangle roomBounds,
        int originX,
        int originY,
        float scaleX,
        float scaleY)
    {
        int x = originX + (int)((world.X - roomBounds.X) * scaleX);
        int y = originY + (int)((world.Y - roomBounds.Y) * scaleY);
        int w = System.Math.Max(1, (int)(world.Width  * scaleX));
        int h = System.Math.Max(1, (int)(world.Height * scaleY));
        return new Rectangle(x, y, w, h);
    }
}