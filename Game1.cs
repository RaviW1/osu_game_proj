using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace osu_game_proj
{
    public class Game1 : Game
    {
        // Singleton so static command classes (CycleEnemy, CycleBlock, etc.) can
        // reach back into the running game without passing Game1 references everywhere.
        private static Game1 instance;

        // --- Rendering ---
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private IScene _currentScene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        // Sets up input controllers and key bindings. Most bindings live in
        // BindKeyboardKeys, item-cycling is bound here because
        // it depends on the ItemManager instance.
        protected override void Initialize()
        {
            _currentScene = new GameScene(GraphicsDevice, Content, this);
            _currentScene.Initialize();
            base.Initialize();
        }

        // Loads all textures and creates game objects via helper methods.
        // Shared textures (fireballTexture, pixelTexture) are loaded first since
        // multiple helpers depend on them.

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentScene.Load();
        }


        // Per-frame game logic: update entities, process input, then run collision checks.
        // All collision detection is handled by PhysicsHelper — including
        // projectile-vs-player, player-projectile-vs-enemy, melee-vs-enemy, and tile collisions.

        protected override void Update(GameTime gameTime)
        {
            

            _currentScene.Update(gameTime);
            base.Update(gameTime);
        }


        // Draw order: level tiles -> active enemy/block -> geos -> player -> UI overlays.
        // Everything is drawn in a single SpriteBatch pass.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 8, 16));
            _currentScene.Draw(_spriteBatch, gameTime);
            base.Draw(gameTime);
        }


        // Resets all game state to its initial configuration. Reuses the same
        // Create* helpers as LoadContent to avoid duplication.
        // Bound to the R key via ResetCommand
        public void Reset()
        {
        }

        // --- Static cycling methods (called by command classes via the singleton) ---

    }
}
