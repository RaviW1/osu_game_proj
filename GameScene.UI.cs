using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public partial class GameScene
{
    private const int CharmSize = 64;
    private const int CharmSpacing = 20;
    private const int WaywardCompassIndex = 2;

    private float _charmDenyTimer = 0f;
    private const float CharmDenyDuration = 2f;
    private Rectangle _closeButtonRect;
    private const int CloseBtnSize = 32;

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

        int gap = 20;

        string replayText = "Replay";
        Vector2 replaySize = font.MeasureString(replayText);
        int replayW = (int)replaySize.X + 40, replayH = (int)replaySize.Y + 20;

        string menuText = "Main Menu";
        Vector2 menuSize = font.MeasureString(menuText);
        int menuW = (int)menuSize.X + 40, menuH = (int)menuSize.Y + 20;

        string quitText = "Quit";
        Vector2 quitSize = font.MeasureString(quitText);
        int quitW = (int)quitSize.X + 40, quitH = (int)quitSize.Y + 20;

        int totalW = replayW + gap + menuW + gap + quitW;
        int startX = (vw - totalW) / 2;
        int btnY = (int)(vh * 0.5f);

        // Replay
        _restartButtonRect = new Rectangle(startX, btnY, replayW, replayH);
        spriteBatch.Draw(pixelTexture, _restartButtonRect, Color.DarkGreen * _winAlpha);
        spriteBatch.DrawString(font, replayText,
            new Vector2(_restartButtonRect.X + (_restartButtonRect.Width - replaySize.X) / 2f,
                        _restartButtonRect.Y + (_restartButtonRect.Height - replaySize.Y) / 2f),
            Color.White * _winAlpha);

        // Main Menu
        _mainMenuButtonRect = new Rectangle(startX + replayW + gap, btnY, menuW, menuH);
        spriteBatch.Draw(pixelTexture, _mainMenuButtonRect, Color.DarkBlue * _winAlpha);
        spriteBatch.DrawString(font, menuText,
            new Vector2(_mainMenuButtonRect.X + (_mainMenuButtonRect.Width - menuSize.X) / 2f,
                        _mainMenuButtonRect.Y + (_mainMenuButtonRect.Height - menuSize.Y) / 2f),
            Color.White * _winAlpha);

        // Quit
        _quitButtonRect = new Rectangle(startX + replayW + gap + menuW + gap, btnY, quitW, quitH);
        spriteBatch.Draw(pixelTexture, _quitButtonRect, Color.DarkRed * _winAlpha);
        spriteBatch.DrawString(font, quitText,
            new Vector2(_quitButtonRect.X + (_quitButtonRect.Width - quitSize.X) / 2f,
                        _quitButtonRect.Y + (_quitButtonRect.Height - quitSize.Y) / 2f),
            Color.White * _winAlpha);
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
        spriteBatch.DrawString(font, resumeText,
            new Vector2((vw - resumeSize.X) / 2f, vh * 0.5f), Color.LightGray);

        string menuText = "Main Menu";
        Vector2 menuSize = font.MeasureString(menuText);
        int menuW = (int)menuSize.X + 40, menuH = (int)menuSize.Y + 20;
        _mainMenuButtonRect = new Rectangle((vw - menuW) / 2, (int)(vh * 0.62f), menuW, menuH);
        spriteBatch.Draw(pixelTexture, _mainMenuButtonRect, Color.DarkBlue);
        spriteBatch.DrawString(font, menuText,
            new Vector2(_mainMenuButtonRect.X + (_mainMenuButtonRect.Width - menuSize.X) / 2f,
                        _mainMenuButtonRect.Y + (_mainMenuButtonRect.Height - menuSize.Y) / 2f),
            Color.White);
    }

    private bool IsCharmVisible(int index)
    {
        if (index == WaywardCompassIndex && !player.HasWaywardCompass) return false;
        return true;
    }

    private int GetVisibleCharmCount()
    {
        int count = 0;
        for (int i = 0; i < itemManager.Count; i++)
            if (IsCharmVisible(i)) count++;
        return count;
    }

    private Rectangle GetCharmRect(int index)
    {
        int vw = _graphics.Viewport.Width;
        int visCount = GetVisibleCharmCount();
        int vi = 0;
        for (int j = 0; j < index; j++)
            if (IsCharmVisible(j)) vi++;
        int totalW = visCount * CharmSize + (visCount - 1) * CharmSpacing;
        int startX = (vw - totalW) / 2 + vi * (CharmSize + CharmSpacing);
        int y = _graphics.Viewport.Height / 2 - CharmSize / 2;
        return new Rectangle(startX, y, CharmSize, CharmSize);
    }

    private void HandleCharmClick(Point mousePos)
    {
        itemManager.VisibleCount = GetVisibleCharmCount();
        for (int i = 0; i < itemManager.Count; i++)
        {
            if (!IsCharmVisible(i)) continue;
            if (GetCharmRect(i).Contains(mousePos))
            {
                if (!itemManager.IsEquipped(i) && itemManager.EquippedCount >= itemManager.MaxEquipped)
                {
                    _charmDenyTimer = CharmDenyDuration;
                }
                else
                {
                    itemManager.ToggleItem(i, player);
                }
                break;
            }
        }
    }

    private void DrawCloseButton(SpriteBatch spriteBatch)
    {
        _closeButtonRect = new Rectangle(12, 12, CloseBtnSize, CloseBtnSize);
        bool hover = _closeButtonRect.Contains(Microsoft.Xna.Framework.Input.Mouse.GetState().Position);
        Color bg = hover ? Color.Lerp(Color.DarkRed, Color.White, 0.2f) : new Color(60, 20, 20);
        spriteBatch.Draw(pixelTexture, _closeButtonRect, bg);
        string x = "X";
        Vector2 xSize = font.MeasureString(x);
        spriteBatch.DrawString(font, x,
            new Vector2(_closeButtonRect.X + (_closeButtonRect.Width - xSize.X) / 2f,
                        _closeButtonRect.Y + (_closeButtonRect.Height - xSize.Y) / 2f),
            Color.White);
    }

    private void DrawCharmInventory(SpriteBatch spriteBatch)
    {
        itemManager.VisibleCount = GetVisibleCharmCount();
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.6f);
        DrawCloseButton(spriteBatch);

        string title = "Charms Inventory";
        float titleScale = 1.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        Vector2 titlePos = new Vector2((vw - titleSize.X) / 2f, vh * 0.15f);
        spriteBatch.DrawString(font, title, titlePos, Color.White,
            0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        for (int i = 0; i < itemManager.Count; i++)
        {
            if (!IsCharmVisible(i)) continue;
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

        string slotInfo = $"Charm slots: {itemManager.EquippedCount} / {itemManager.MaxEquipped}";
        Vector2 slotInfoSize = font.MeasureString(slotInfo);
        Color slotColor = itemManager.EquippedCount >= itemManager.MaxEquipped ? Color.IndianRed : Color.LimeGreen;
        spriteBatch.DrawString(font, slotInfo, new Vector2((vw - slotInfoSize.X) / 2f, vh * 0.68f), slotColor);

        if (_charmDenyTimer > 0f)
        {
            float alpha = MathHelper.Clamp(_charmDenyTimer / 0.5f, 0f, 1f);
            string warn = "No slots available! Unequip a charm first.";
            Vector2 warnSize = font.MeasureString(warn);
            spriteBatch.DrawString(font, warn,
                new Vector2((vw - warnSize.X) / 2f, vh * 0.58f), Color.Red * alpha);
        }

        string equipHint = itemManager.EquippedCount >= itemManager.MaxEquipped
            ? "Unequip a charm first to equip another"
            : "Click to equip / unequip";
        Vector2 equipHintSize = font.MeasureString(equipHint);
        spriteBatch.DrawString(font, equipHint, new Vector2((vw - equipHintSize.X) / 2f, vh * 0.76f), Color.LightGray);

        if (GetVisibleCharmCount() < itemManager.Count)
        {
            string slotTip = "Tip: Buying more charms unlocks more active slots";
            Vector2 slotTipSize = font.MeasureString(slotTip);
            spriteBatch.DrawString(font, slotTip, new Vector2((vw - slotTipSize.X) / 2f, vh * 0.88f), Color.DarkGoldenrod);
        }

        string hint = "Press I to close";
        Vector2 hintSize = font.MeasureString(hint);
        spriteBatch.DrawString(font, hint, new Vector2((vw - hintSize.X) / 2f, vh * 0.82f), Color.Gray);
    }

    private void DrawShopHUD(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.7f);
        DrawCloseButton(spriteBatch);

        string title = "Shop";
        float titleScale = 2f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(font, title, new Vector2((vw - titleSize.X) / 2f, vh * 0.1f),
            Color.Gold, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        TextureItem compass = (TextureItem)itemManager.GetItem(WaywardCompassIndex);
        Texture2D tex = compass.Texture;
        int iconSize = 96;
        float iconScale = Math.Min((float)iconSize / tex.Width, (float)iconSize / tex.Height);
        int drawW = (int)(tex.Width * iconScale);
        int drawH = (int)(tex.Height * iconScale);
        int iconX = (vw - drawW) / 2;
        int iconY = (int)(vh * 0.28f);
        spriteBatch.Draw(tex, new Vector2(iconX, iconY), null, Color.White,
            0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);

        string name = "Wayward Compass";
        Vector2 nameSize = font.MeasureString(name);
        spriteBatch.DrawString(font, name,
            new Vector2((vw - nameSize.X) / 2f, iconY + drawH + 12), Color.White);

        string desc = "Reveals the minimap when equipped";
        Vector2 descSize = font.MeasureString(desc);
        spriteBatch.DrawString(font, desc,
            new Vector2((vw - descSize.X) / 2f, iconY + drawH + 40), Color.LightGray);

        if (player.HasWaywardCompass)
        {
            string owned = "Purchased";
            Vector2 ownedSize = font.MeasureString(owned);
            spriteBatch.DrawString(font, owned,
                new Vector2((vw - ownedSize.X) / 2f, vh * 0.62f), Color.LimeGreen);
            _shopBuyButtonRect = Rectangle.Empty;
        }
        else
        {
            string btnText = "Buy - 10 Geo";
            Vector2 btnSize = font.MeasureString(btnText);
            int btnW = (int)btnSize.X + 20;
            int btnH = (int)btnSize.Y + 10;
            _shopBuyButtonRect = new Rectangle((vw - btnW) / 2, (int)(vh * 0.68f), btnW, btnH);

            bool canAfford = player.GeoCount >= 10;
            Color btnColor = canAfford ? Color.DarkGreen : Color.DarkGray;
            spriteBatch.Draw(pixelTexture, _shopBuyButtonRect, btnColor);

            Color textColor = canAfford ? Color.White : Color.Gray;
            Vector2 textPos = new Vector2(
                _shopBuyButtonRect.X + (_shopBuyButtonRect.Width - btnSize.X) / 2f,
                _shopBuyButtonRect.Y + (_shopBuyButtonRect.Height - btnSize.Y) / 2f);
            spriteBatch.DrawString(font, btnText, textPos, textColor);
        }

        string closeHint = "Press B to close";
        Vector2 closeSize = font.MeasureString(closeHint);
        spriteBatch.DrawString(font, closeHint,
            new Vector2((vw - closeSize.X) / 2f, vh * 0.82f), Color.Gray);
    }

    private void HandleShopClick(Point mousePos)
    {
        if (player.HasWaywardCompass) return;
        if (player.GeoCount < 10) return;
        if (!_shopBuyButtonRect.Contains(mousePos)) return;

        player.GeoCount -= 10;
        player.HasWaywardCompass = true;
        itemManager.VisibleCount = GetVisibleCharmCount();
    }
}
