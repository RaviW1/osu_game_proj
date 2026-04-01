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
        private SpriteFont font;
        private Texture2D pixelTexture;
        // Shared by Aspid enemies and the player's ShootFireball; loaded once and
        // passed into both CreateEnemies() and CreatePlayer().
        // Change this if you want to use a different fireball texture.
        private Texture2D fireballTexture;

        // --- Game objects ---
        private Player player;
        private List<ISprite> enemies;
        private List<ISprite> blocks;
        private LevelsHandler levels;
        private ItemManager itemManager;
        private AbilityBar abilityBar;

        // --- Input ---
        private KeyboardController keyboard;
        private MouseController mouse;

        // Indices into the enemies / blocks lists; only one of each is active at a time.
        // If you want to use more than one enemy or block, you can change this.
        private int currentEnemyIndex;
        private int currentBlockIndex;

        private List<Geo> geos;
        private List<Geo> geosLevel1;
        private List<Geo> geosLevel2;
        private Texture2D geoTexture;
        private List<TileInformation> generateTileInfo;
        private LoadLevelFile level1FileLoader;
        private TileGenerator tileGenObj1;
        private LoadLevelFile level2FileLoader;
        private TileGenerator tileGenObj2;
        private TileGenerator drawTilesGen;

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
            keyboard = new KeyboardController();
            new BindKeyboardKeys(keyboard).bindKeys(this);

            itemManager = new ItemManager(0.4f);
            keyboard.BindPress(Keys.U, new CycleItemCommand(-1, dir => itemManager.CycleItem(dir, player)));
            keyboard.BindPress(Keys.I, new CycleItemCommand(1, dir => itemManager.CycleItem(dir, player)));

            mouse = new MouseController(this,
                new CycleStageCommand(-1), new CycleStageCommand(1),
                new CycleStageCommand(-1), new CycleStageCommand(1),
                new CycleStageCommand(-1));

            base.Initialize();
        }

        // Loads all textures and creates game objects via helper methods.
        // Shared textures (fireballTexture, pixelTexture) are loaded first since
        // multiple helpers depend on them.

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
            pixelTexture = CreatePixelTexture();
            fireballTexture = Content.Load<Texture2D>("fireball");

            enemies = CreateEnemies();
            blocks = CreateBlocks();
            player = CreatePlayer();
            abilityBar = CreateAbilityBar();
            LoadItems();

            levels = new LevelsHandler();
            levels.LoadLevelTiles(Content);
            _currentScene = new GameScene(GraphicsDevice, Content, this);
            _currentScene.Load();
        }


        // Per-frame game logic: update entities, process input, then run collision checks.
        // All collision detection is handled by PhysicsHelper — including
        // projectile-vs-player, player-projectile-vs-enemy, melee-vs-enemy, and tile collisions.

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update active entities
            if (enemies.Count > 0)
                enemies[currentEnemyIndex].Update(gameTime);
            if (blocks.Count > 0)
                blocks[currentBlockIndex].Update(gameTime);

            player.Update(gameTime);
            itemManager.Update(gameTime);

            // Gather and execute input commands
            ProcessInput(gameTime);

            // Collision detection (geo pickups are checked even without tiles)
            PhysicsHelper.CheckPlayerGeosCollisions(player, levels.currentGeos, gameTime);
            if (levels.currentTilesGen != null)
            {
                PhysicsHelper.CheckCollisions(player, levels.currentTilesGen);
                PhysicsHelper.CheckEnemyCollisions(player, enemies, currentEnemyIndex, levels.currentTilesGen);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentScene.Update(gameTime);
            base.Update(gameTime);
        }


        // Draw order: level tiles -> active enemy/block -> geos -> player -> UI overlays.
        // Everything is drawn in a single SpriteBatch pass.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            levels.Draw(_spriteBatch);

            if (enemies.Count > 0)
                enemies[currentEnemyIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);
            if (blocks.Count > 0)
                blocks[currentBlockIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);

            foreach (var geo in levels.currentGeos)
                geo.Draw(_spriteBatch);

            player.Draw(_spriteBatch, gameTime);
            itemManager.Draw(_spriteBatch);
            abilityBar.Draw(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            HUD.DrawHUD(player, _spriteBatch, GraphicsDevice.Viewport.Width, font);

            _currentScene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }


        // Resets all game state to its initial configuration. Reuses the same
        // Create* helpers as LoadContent to avoid duplication.
        // Bound to the R key via ResetCommand
        public void Reset()
        {
            enemies = CreateEnemies();
            player = CreatePlayer();

            itemManager = new ItemManager(0.4f);
            LoadItems();

            levels.ClearGeos();
            currentEnemyIndex = 0;
            currentBlockIndex = 0;
        }

        // --- Static cycling methods (called by command classes via the singleton) ---

        // Wraps the enemy index with modular arithmetic so it loops around.
        public static void CycleEnemy(int direction)
        {
            if (instance.enemies.Count == 0) return;
            instance.currentEnemyIndex += direction;
            if (instance.currentEnemyIndex < 0)
                instance.currentEnemyIndex = instance.enemies.Count - 1;
            else if (instance.currentEnemyIndex >= instance.enemies.Count)
                instance.currentEnemyIndex = 0;
        }

        // Wraps the block index with modular arithmetic so it loops around.
        public static void CycleBlock(int direction)
        {
            if (instance.blocks.Count == 0) return;
            instance.currentBlockIndex += direction;
            if (instance.currentBlockIndex < 0)
                instance.currentBlockIndex = instance.blocks.Count - 1;
            else if (instance.currentBlockIndex >= instance.blocks.Count)
                instance.currentBlockIndex = 0;
        }

        public static void CycleStage(int direction)
        {
            instance.levels.CycleStage(direction);
        }

        // =====================================================================
        //  Private helpers — each builds one logical group of game objects.
        //  ContentManager caches textures, so duplicate Load<T> calls are cheap.
        // =====================================================================

        // Polls both controllers and executes every queued command against the player.
        private void ProcessInput(GameTime gameTime)
        {
            foreach (ICommand cmd in keyboard.GetCommands(gameTime))
                cmd.Execute(player, gameTime);
            foreach (ICommand cmd in mouse.GetCommands(gameTime))
                cmd.Execute(player, gameTime);
        }

        private List<ISprite> CreateEnemies()
        {
            Texture2D booflyTex = Content.Load<Texture2D>("Enemy Sprites\\boofly");
            Texture2D aspidTex = Content.Load<Texture2D>("Enemy Sprites\\aspid_hunter");
            Texture2D huskBullyTex = Content.Load<Texture2D>("Enemy Sprites\\husk_bully");

            return new List<ISprite>
            {
                new Boofly(booflyTex, new System.Numerics.Vector2(500, 50)),
                new Aspid(aspidTex, fireballTexture, new System.Numerics.Vector2(500, 50)),
                new HuskBully(huskBullyTex, new System.Numerics.Vector2(100, 360))
            };
        }

        private List<ISprite> CreateBlocks()
        {
            Texture2D spikeTex = Content.Load<Texture2D>("spike_back");
            Texture2D fungalSpikeTex = Content.Load<Texture2D>("fungd_spikes_01");

            return new List<ISprite>
            {
                new MapBlock(spikeTex, new System.Numerics.Vector2(50, 50)),
                new MapBlock(fungalSpikeTex, new System.Numerics.Vector2(50, 50))
            };
        }

        private Player CreatePlayer()
        {
            var textures = new Dictionary<string, Texture2D>
            {
                { "Walking", Content.Load<Texture2D>("hollow_knight_walking") },
                { "Jumping", Content.Load<Texture2D>("knight_jumping") },
                { "Attacking", Content.Load<Texture2D>("knight_attack") },
                { "Attack", Content.Load<Texture2D>("hollow_knight_attack") }
            };
            return new Player(textures, fireballTexture, new Vector2(350, 370));
        }


        // Builds the bottom-of-screen ability bar. Each ability needs both an icon
        // texture and a source rectangle that crops the correct region from its sprite sheet.
        private AbilityBar CreateAbilityBar()
        {
            Texture2D playerTex = Content.Load<Texture2D>("hollow_knight_walking");

            var icons = new Dictionary<string, Texture2D>
            {
                { "Attack", Content.Load<Texture2D>("hollow_knight_attack") },
                { "Fireball", fireballTexture },
                { "Heal", playerTex }
            };

            var sourceRects = new Dictionary<string, Rectangle?>
            {
                { "Attack", new Rectangle(896, 0, 128, 128) },
                { "Fireball", new Rectangle(0, 0, fireballTexture.Width / 2, fireballTexture.Height / 2) },
                { "Heal", new Rectangle(0, 0, playerTex.Width / 8, playerTex.Height) }
            };

            return new AbilityBar(pixelTexture, icons, sourceRects, Vector2.Zero);
        }


        // Registers equippable items. Each TextureItem takes an onEquip and onUnequip
        // callback so item effects are self-contained.
        private void LoadItems()
        {
            Texture2D heartTex = Content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
            Texture2D dashTex = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");

            itemManager.AddItem(
                new TextureItem(0, heartTex, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2),
                new Vector2(10, 10));
            itemManager.AddItem(
                new TextureItem(1, dashTex, p => p.CanDash = true, p => p.CanDash = false),
                new Vector2(100, 10));
        }

        //Creates a 1x1 white pixel used as a building block for UI rectangles.
        private Texture2D CreatePixelTexture()
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
=======
            _currentScene.Draw(_spriteBatch, gameTime);
            base.Draw(gameTime);
        }
>>>>>>> b353c20 (camera system and bug fixes)
    }
}
