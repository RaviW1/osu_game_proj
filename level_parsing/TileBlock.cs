using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class TileBlock
{
    private Texture2D texture;
    private Rectangle destRect;
    public Rectangle bounds;
    public bool isCollideable;
    public bool isHarmful;
    public string tileType;  

    public TileBlock(Texture2D texture, Rectangle destRectangle, bool isCollideable, bool isHarmful, string tileType = "")
    {
        this.texture = texture;
        this.destRect = destRectangle;
        this.bounds = destRectangle;
        this.isCollideable = isCollideable;
        this.isHarmful = isHarmful;
        this.tileType = tileType;  
    }

    public void Update() { }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            this.texture,
            this.destRect,
            null,
            Color.White,
            0.0f,
            new Vector2(0, 0),
            SpriteEffects.None,
            0.0f);
    }
}