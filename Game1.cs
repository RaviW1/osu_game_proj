using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


//Main class game1.cs
namespace osu_game_proj
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player player;
        private KeyboardController keyboard;
        private MouseController mouse;
        private ItemManager itemManager;
        private List<ISprite> enemies;
        private List<ISprite> blocks;
        private int currentEnemyIndex = 0;
        private int currentBlockIndex = 0;
        private static Game1 instance;
        private AbilityBar abilityBar;
        private Texture2D pixelTexture;
        private SpriteFont font;
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
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // ------------------------------
            // KEYBINDINGS (client code)
            // Assumes you already created:
            //   KeyboardController keyboard = new KeyboardController();
            // And you have these command classes available:
            //   MovementAxisCommand, VerticalAxisCommand, JumpPressedCommand, JumpReleasedCommand
            // ------------------------------

            keyboard = new KeyboardController();

            BindKeyboardKeys keyBindObj = new BindKeyboardKeys(keyboard);
            keyBindObj.bindKeys(this);

            // ---- Item cycling (u = previous, i = next) ----
            itemManager = new ItemManager(0.4f);
            var cyclePrevItemCmd = new CycleItemCommand(-1, (dir) => itemManager.CycleItem(dir, player));
            var cycleNextItemCmd = new CycleItemCommand(1, (dir) => itemManager.CycleItem(dir, player));
            keyboard.BindPress(Keys.U, cyclePrevItemCmd);
            keyboard.BindPress(Keys.I, cycleNextItemCmd);

            mouse = new MouseController(this, new CycleStageCommand(-1), new CycleStageCommand(1), new CycleStageCommand(-1), new CycleStageCommand(1), new CycleStageCommand(-1));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load enemy textures
            enemies = new List<ISprite>();
            Texture2D enemyTexture = Content.Load<Texture2D>("boofly");
            enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D aspidTexture = Content.Load<Texture2D>("Aspid");
            Texture2D fireballTexture = Content.Load<Texture2D>("fireball");
            enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));
            // Create pixel texture for UI
            pixelTexture = CreatePixelTexture();

            // Create ability icons dictionary
            var abilityIcons = new Dictionary<string, Texture2D>();
            abilityIcons.Add("Attack", Content.Load<Texture2D>("hollow_knight_attack"));
            abilityIcons.Add("Fireball", Content.Load<Texture2D>("fireball"));
            abilityIcons.Add("Heal", Content.Load<Texture2D>("hollow_knight_walking"));

            // Define source rectangles for each icon 
            var iconSourceRects = new Dictionary<string, Rectangle?>();

            // Attack sprite 
            iconSourceRects.Add("Attack", new Rectangle(896, 0, 128, 128));

            // Fireball sprite 
            iconSourceRects.Add("Fireball", new Rectangle(0, 0, fireballTexture.Width / 2, fireballTexture.Height / 2));
            Texture2D playerTexture = Content.Load<Texture2D>("hollow_knight_walking");
            iconSourceRects.Add("Heal", new Rectangle(0, 0, playerTexture.Width / 8, playerTexture.Height));

            // Create ability bar
            abilityBar = new AbilityBar(pixelTexture, abilityIcons, iconSourceRects, Vector2.Zero);


            // Load block textures
            blocks = new List<ISprite>();
            Texture2D spikeTexture = Content.Load<Texture2D>("spike_back");
            blocks.Add(new MapBlock(spikeTexture, new System.Numerics.Vector2(50, 50)));
            Texture2D fungalSpikeTexture = Content.Load<Texture2D>("fungd_spikes_01");
            blocks.Add(new MapBlock(fungalSpikeTexture, new System.Numerics.Vector2(50, 50)));

            List<TileInformation> generateTileInfo = new List<TileInformation>();
            level1FileLoader = new LoadLevelFile();
            level1FileLoader.LoadFile("level_files/test_level.xml", generateTileInfo);

            tileGenObj1 = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj1.LoadTileTextures(Content);

            generateTileInfo.Clear();

            // Load Level 2
            level2FileLoader = new LoadLevelFile();
            level2FileLoader.LoadFile("level_files/test_level2.xml", generateTileInfo);

            tileGenObj2 = new TileGenerator(new List<TileInformation>(generateTileInfo));
            tileGenObj2.LoadTileTextures(Content);

            drawTilesGen = tileGenObj1;

            geoTexture = Content.Load<Texture2D>("Geo - HUD_coin_shop");
            geosLevel1 = new List<Geo>();
            geosLevel2 = new List<Geo>();
            Geo.PlaceGeosOnPlatforms(tileGenObj1, geosLevel1, geoTexture);
            Geo.PlaceGeosOnPlatforms(tileGenObj2, geosLevel2, geoTexture);
            geos = geosLevel1;

            // TODO: use this.Content to load your game content here

            // Load Player Textures
            var playerTextures = new Dictionary<string, Texture2D>();
            playerTextures.Add("Walking", Content.Load<Texture2D>("hollow_knight_walking"));
            playerTextures.Add("Jumping", Content.Load<Texture2D>("knight_jumping"));
            playerTextures.Add("Attacking", Content.Load<Texture2D>("knight_attack"));

            // create new player object
            player = new Player(playerTextures, fireballTexture, new Vector2(350, 370));

            // Load item textures and add to item manager
            // ID 0: Unbreakable Heart (+2 HP on select), ID 1: Dashmaster (canDash on select)
            Texture2D unbreakableHeart = Content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
            Texture2D dashmaster = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
            itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2), new Vector2(10, 10));
            itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

            playerTextures.Add("Attack", Content.Load<Texture2D>("hollow_knight_attack"));

            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // TODO: Break into physics statics class


            if (enemies.Count > 0)
            {
                enemies[currentEnemyIndex].Update();
            }
            PhysicsHelper.CheckEnemyCollisions(player, enemies, currentBlockIndex);
            PhysicsHelper.CheckPlayerGeosCollisions(player, geos, gameTime);


            if (blocks.Count > 0)
            {
                blocks[currentBlockIndex].Update();
            }

            List<ICommand> currentCommands = keyboard.GetCommands(gameTime);
            foreach (ICommand command in currentCommands)
            {
                command.Execute(player, gameTime);
            }

            List<ICommand> currentMouseCommands = mouse.GetCommands(gameTime);
            foreach (ICommand command in currentMouseCommands)
            {
                command.Execute(player, gameTime);
            }


            itemManager.Update(gameTime);

            if (instance.drawTilesGen != null)
            {
                PhysicsHelper.CheckCollisions(player, instance.drawTilesGen);
            }
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            drawTilesGen.Draw(_spriteBatch);

            // TODO: break this out into a seperate class
            if (enemies.Count > 0)
            {
                enemies[currentEnemyIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);
            }

            if (blocks.Count > 0)
            {
                blocks[currentBlockIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);
            }
            abilityBar.Draw(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);


            foreach (var geo in geos)
            {
                geo.Draw(_spriteBatch);
            }

            player.Draw(_spriteBatch, gameTime);
            itemManager.Draw(_spriteBatch);

            // TODO: Break HUD drawing into seperate class
            int viewWidth = GraphicsDevice.Viewport.Width;

            HUD.DrawHUD(player, _spriteBatch, viewWidth, font);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
        // FOR TESTING
        public static void CycleEnemy(int direction)
        {
            if (instance.enemies.Count == 0) return;

            instance.currentEnemyIndex += direction;

            if (instance.currentEnemyIndex < 0)
                instance.currentEnemyIndex = instance.enemies.Count - 1;
            else if (instance.currentEnemyIndex >= instance.enemies.Count)
                instance.currentEnemyIndex = 0;
        }

        public static void CycleBlock(int direction)
        {
            if (instance.blocks.Count == 0)
            {
                return;
            }

            instance.currentBlockIndex += direction;

            if (instance.currentBlockIndex < 0)
            {
                instance.currentBlockIndex = instance.blocks.Count - 1;
            }
            else if (instance.currentBlockIndex >= instance.blocks.Count)
            {
                instance.currentBlockIndex = 0;
            }
        }
        public static void CycleStage(int direction)
        {
            if (direction == -1)
            {
                instance.drawTilesGen = instance.tileGenObj1;
                instance.geos = instance.geosLevel1;
            }
            else if (direction == 1)
            {
                instance.drawTilesGen = instance.tileGenObj2;
                instance.geos = instance.geosLevel2;
            }
        }

        public void Reset()
        {
            // Reset player with all textures
            var playerTextures = new Dictionary<string, Texture2D>();
            playerTextures.Add("Walking", Content.Load<Texture2D>("hollow_knight_walking"));
            playerTextures.Add("Jumping", Content.Load<Texture2D>("knight_jumping"));
            playerTextures.Add("Attacking", Content.Load<Texture2D>("knight_attack"));
            playerTextures.Add("Attack", Content.Load<Texture2D>("hollow_knight_attack"));

            Texture2D fireballTexture = Content.Load<Texture2D>("fireball");
            player = new Player(playerTextures, fireballTexture, new Vector2(350, 370));


            // Reset enemies
            enemies.Clear();
            Texture2D enemyTexture = Content.Load<Texture2D>("boofly");
            enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D aspidTexture = Content.Load<Texture2D>("Aspid");
            enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));

            // Reset item manager
            itemManager = new ItemManager(0.4f);
            Texture2D unbreakableHeart = Content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
            Texture2D dashmaster = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
            itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2), new Vector2(10, 10));
            itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

            // Reset geos for both levels
            geosLevel1.Clear();
            geosLevel2.Clear();

            Geo.PlaceGeosOnPlatforms(tileGenObj1, geosLevel1, geoTexture);
            Geo.PlaceGeosOnPlatforms(tileGenObj2, geosLevel2, geoTexture);
            geos = (drawTilesGen == tileGenObj1) ? geosLevel1 : geosLevel2;

            // Reset indices
            currentEnemyIndex = 0;
            currentBlockIndex = 0;
        }

        private Texture2D CreatePixelTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }

        // DO NOT USE
        // WILL BE REMOVED SOON
        public static List<Rectangle> GetCurrentLevelColliders()
        {
            var rects = new List<Rectangle>();

            foreach (var tile in instance.drawTilesGen.generateTileInfo)
            {
                // Skip decorative background tiles
                if (tile.tileType == "level1_background" ||
                    tile.tileType == "left_rocks_wall" ||
                    tile.tileType == "right_cave_wall" ||
                    tile.tileType == "top_cave_wall")
                    continue;

                rects.Add(tile.destRectangle);
            }

            return rects;
        }
    }
}
