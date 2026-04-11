using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public partial class GameScene
{
    private const int CharmSize = 64;
    private const int CharmSpacing = 20;

    private void DrawSoulMeter(SpriteBatch spriteBatch)
    {
        int cellW = _soulMeterTexture.Width / 2;
        int cellH = _soulMeterTexture.Height / 3;
        int yOffset = cellH - 15;
        int drawW = cellW * 2 / 3;
        int drawH = cellH * 2 / 3;
        Rectangle sourceRect = new Rectangle(0, yOffset, cellW, cellH);
        Rectangle destRect = new Rectangle(10, 10, drawW, drawH);
        spriteBatch.Draw(_soulMeterTexture, destRect, sourceRect, Color.White);
    }

    private void DrawHPBar(SpriteBatch spriteBatch)
    {
        int framW = _hpMaskTexture.Width / 14;
        int framH = _hpMaskTexture.Height / 14;
        int pad = 6;

        Rectangle fullSrc = new Rectangle(pad, pad + 3, framW - pad * 2, framH - pad);
        Rectangle emptySrc = new Rectangle(pad, framH * 2 + pad + 8, framW - pad * 2, framH - pad);

        int soulDrawW = (_soulMeterTexture.Width / 2) * 2 / 3;
        int startX = 10 + soulDrawW / 2;
        int startY = 10 + (_soulMeterTexture.Height / 3) * 2 / 3 * 2 / 3;
        int drawSize = 30;
        int spacing = 2;

        for (int i = 0; i < player.MaxPlayerHealth; i++)
        {
            Rectangle dest = new Rectangle(startX + i * (drawSize + spacing), startY, drawSize, drawSize);
            if (i < player.PlayerHealth)
                spriteBatch.Draw(_hpMaskTexture, dest, fullSrc, Color.White);
            else
                spriteBatch.Draw(_hpMaskTexture, dest, emptySrc, Color.White);
        }
    }

    private void DrawGameOver(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;
        Color tint = Color.White * _gameOverAlpha;

        spriteBatch.Draw(_gameOverTexture, new Rectangle(0, 0, vw, vh), tint);

        string title = "Game Over";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        Vector2 titlePos = new Vector2((vw - titleSize.X) / 2f, vh * 0.3f);
        spriteBatch.DrawString(font, title, titlePos, Color.White * _gameOverAlpha,
            0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string btnText = "Restart";
        Vector2 btnTextSize = font.MeasureString(btnText);
        int btnW = (int)btnTextSize.X + 40;
        int btnH = (int)btnTextSize.Y + 20;
        _restartButtonRect = new Rectangle((vw - btnW) / 2, (int)(vh * 0.5f), btnW, btnH);

        spriteBatch.Draw(pixelTexture, _restartButtonRect, Color.DarkGray * _gameOverAlpha);
        Vector2 btnTextPos = new Vector2(
            _restartButtonRect.X + (_restartButtonRect.Width - btnTextSize.X) / 2f,
            _restartButtonRect.Y + (_restartButtonRect.Height - btnTextSize.Y) / 2f);
        spriteBatch.DrawString(font, btnText, btnTextPos, Color.White * _gameOverAlpha);
    }

    private void DrawWinScreen(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.75f * _winAlpha);

        string title = "You Win!";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(font, title, new Vector2((vw - titleSize.X) / 2f, vh * 0.25f),
            Color.Gold * _winAlpha, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string replayText = "Replay";
        Vector2 replaySize = font.MeasureString(replayText);
        int btnW = (int)replaySize.X + 40, btnH = (int)replaySize.Y + 20;
        _restartButtonRect = new Rectangle((vw / 2) - btnW - 20, (int)(vh * 0.5f), btnW, btnH);
        spriteBatch.Draw(pixelTexture, _restartButtonRect, Color.DarkGreen * _winAlpha);
        spriteBatch.DrawString(font, replayText, new Vector2(_restartButtonRect.X + 20, _restartButtonRect.Y + 10), Color.White * _winAlpha);

        string quitText = "Quit";
        Vector2 quitSize = font.MeasureString(quitText);
        int quitW = (int)quitSize.X + 40, quitH = (int)quitSize.Y + 20;
        _quitButtonRect = new Rectangle((vw / 2) + 20, (int)(vh * 0.5f), quitW, quitH);
        spriteBatch.Draw(pixelTexture, _quitButtonRect, Color.DarkRed * _winAlpha);
        spriteBatch.DrawString(font, quitText, new Vector2(_quitButtonRect.X + 20, _quitButtonRect.Y + 10), Color.White * _winAlpha);
    }

    private void DrawPauseScreen(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.5f);

        string title = "Paused";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(font, title, new Vector2((vw - titleSize.X) / 2f, vh * 0.25f),
            Color.White, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string resumeText = "Press ESC to Resume";
        Vector2 resumeSize = font.MeasureString(resumeText);
        spriteBatch.DrawString(font, resumeText, new Vector2((vw - resumeSize.X) / 2f, vh * 0.5f), Color.LightGray);
    }

    private Rectangle GetCharmRect(int index)
    {
        int vw = _graphics.Viewport.Width;
        int totalW = itemManager.Count * CharmSize + (itemManager.Count - 1) * CharmSpacing;
        int startX = (vw - totalW) / 2 + index * (CharmSize + CharmSpacing);
        int y = _graphics.Viewport.Height / 2 - CharmSize / 2;
        return new Rectangle(startX, y, CharmSize, CharmSize);
    }

    private void HandleCharmClick(Point mousePos)
    {
        for (int i = 0; i < itemManager.Count; i++)
        {
            if (GetCharmRect(i).Contains(mousePos))
            {
                itemManager.ToggleItem(i, player);
                break;
            }
        }
    }

    private void DrawCharmInventory(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.6f);

        string title = "Charms Inventory";
        float titleScale = 1.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        Vector2 titlePos = new Vector2((vw - titleSize.X) / 2f, vh * 0.15f);
        spriteBatch.DrawString(font, title, titlePos, Color.White,
            0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        for (int i = 0; i < itemManager.Count; i++)
        {
            Rectangle rect = GetCharmRect(i);
            bool equipped = itemManager.IsEquipped(i);

            Color bgColor = equipped ? Color.Gold * 0.4f : Color.Gray * 0.3f;
            spriteBatch.Draw(pixelTexture, rect, bgColor);

            TextureItem item = (TextureItem)itemManager.GetItem(i);
            Texture2D tex = item.Texture;
            int pad = 6;
            int innerSize = CharmSize - pad * 2;
            float scale = Math.Min((float)innerSize / tex.Width, (float)innerSize / tex.Height);
            int drawW = (int)(tex.Width * scale);
            int drawH = (int)(tex.Height * scale);
            Vector2 drawPos = new Vector2(
                rect.X + (rect.Width - drawW) / 2f,
                rect.Y + (rect.Height - drawH) / 2f);
            Color tint = equipped ? Color.White : Color.Gray * 0.5f;
            spriteBatch.Draw(tex, drawPos, null, tint, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        string equipHint = "Click to equip / unequip";
        Vector2 equipHintSize = font.MeasureString(equipHint);
        spriteBatch.DrawString(font, equipHint, new Vector2((vw - equipHintSize.X) / 2f, vh * 0.76f), Color.LightGray);

        string hint = "Press I to close";
        Vector2 hintSize = font.MeasureString(hint);
        spriteBatch.DrawString(font, hint, new Vector2((vw - hintSize.X) / 2f, vh * 0.82f), Color.Gray);
    }
}
