using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class HUD
{
    public static void DrawHUD(Player player, SpriteBatch _spriteBatch, int viewWidth, SpriteFont font, Texture2D geoSprite)

    {

        float margin = 10f;
        float lineSpacing = 4f;
        float yOffset = margin;

        string soulText = "SOUL " + player.Soul + " / " + player.SoulLimit;
        Vector2 soulSize = font.MeasureString(soulText);
        _spriteBatch.DrawString(font, soulText, new Vector2(viewWidth - soulSize.X - margin, yOffset), Color.CornflowerBlue);
        yOffset += soulSize.Y + lineSpacing;

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
