using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public interface IRoom
{
    Rectangle Bounds { get; }
    List<TileBlock> Tiles { get; }
    void Load(ContentManager content);
    void Update(GameTime gameTime, Player player);
    void Draw(SpriteBatch spriteBatch);
    void Unload();
    Vector2 GetSpawnPoint(string entryId);
}