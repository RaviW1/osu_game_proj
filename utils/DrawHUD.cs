using Microsoft.Xna.Framework;
using System.Collections.Generic;
using osu_game_proj;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public static class HUD
{
    public static void DrawHUD(Player player, SpriteBatch _spriteBatch, int viewWidth, SpriteFont font, Texture2D geoSprite)

    {

        float margin = 10f;
        float lineSpacing = 4f;
        float yOffset = margin;

        string maxHpText = "Max HP " + player.MaxPlayerHealth;
        Vector2 maxHpSize = font.MeasureString(maxHpText);
        _spriteBatch.DrawString(font, maxHpText, new Vector2(viewWidth - maxHpSize.X - margin, yOffset), Color.White);
        yOffset += maxHpSize.Y + lineSpacing;

        string hpText = "HP " + player.PlayerHealth;
        Vector2 hpSize = font.MeasureString(hpText);
        _spriteBatch.DrawString(font, hpText, new Vector2(viewWidth - hpSize.X - margin, yOffset), Color.White);
        yOffset += hpSize.Y + lineSpacing;

        string dashText = player.CanDash ? "Can Dash" : "Can't Dash";
        Vector2 dashSize = font.MeasureString(dashText);
        _spriteBatch.DrawString(font, dashText, new Vector2(viewWidth - dashSize.X - margin, yOffset), Color.White);
        yOffset += dashSize.Y + lineSpacing;

        string countText = "" + player.GeoCount;
        Vector2 countSize = font.MeasureString(countText);
        int spriteSize = (int)countSize.Y;
        float totalWidth = spriteSize + 4 + countSize.X;
        float geoX = viewWidth - totalWidth - margin;

        Rectangle spriteRect = new Rectangle((int)geoX, (int)yOffset, spriteSize, spriteSize);
        _spriteBatch.Draw(geoSprite, spriteRect, Color.White);
        _spriteBatch.DrawString(font, countText, new Vector2(geoX + spriteSize + 4, yOffset), Color.Gold);

    }
}
