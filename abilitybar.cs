using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

public class AbilityBar
{
    private Texture2D backgroundTexture;
    private Dictionary<string, Texture2D> abilityIcons;
    private Dictionary<string, Rectangle?> iconSourceRects; // ADD THIS
    private Vector2 position;
    private int slotCount = 8;
    private int slotSize = 16;
    private int slotSpacing = 3;
    
    public AbilityBar(Texture2D background, Dictionary<string, Texture2D> icons, Dictionary<string, Rectangle?> sourceRects, Vector2 position)
    {
        this.backgroundTexture = background;
        this.abilityIcons = icons;
        this.iconSourceRects = sourceRects; 
        this.position = position;
    }
    
    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        // Draw background bar at bottom center
        int barWidth = (slotSize + slotSpacing) * slotCount + slotSpacing;
        int barHeight = slotSize + slotSpacing * 2;
        int barX = (screenWidth - barWidth) / 2;
        int barY = screenHeight - barHeight - 20;
        
        // Draw semi-transparent background
        Rectangle barRect = new Rectangle(barX, barY, barWidth, barHeight);
        spriteBatch.Draw(backgroundTexture, barRect, Color.Black * 0.7f);
        
        // Draw ability slots
        int currentX = barX + slotSpacing;
        int slotY = barY + slotSpacing;
        
        for (int i = 0; i < slotCount; i++)
        {
            // Draw slot background
            Rectangle slotRect = new Rectangle(currentX, slotY, slotSize, slotSize);
            spriteBatch.Draw(backgroundTexture, slotRect, Color.Gray * 0.5f);
            
            // Draw ability icon if available
            if (i < abilityIcons.Count)
            {
                var iconKey = abilityIcons.ElementAt(i).Key;
                var icon = abilityIcons.ElementAt(i).Value;
                
                if (icon != null)
                {
                    // Get source rectangle if available
                    Rectangle? sourceRect = null;
                    if (iconSourceRects != null && iconSourceRects.ContainsKey(iconKey))
                    {
                        sourceRect = iconSourceRects[iconKey];
                    }
                    
                    spriteBatch.Draw(icon, slotRect, sourceRect, Color.White);
                }
            }
            
            currentX += slotSize + slotSpacing;
        }
    }
}