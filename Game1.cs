using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


//Main class game1.cs
namespace osu_game_proj
{
    public class Game1 : Game
    {
        // TODO: too many fields here
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
        private LevelsHandler levels;
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

            //TODO: Pull this out of Game1
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

            // Test
            Texture2D elderbug = Content.Load<Texture2D>("NPC Sprites\\Elderbug");

            // Load enemy textures
            enemies = new List<ISprite>();
            Texture2D enemyTexture = Content.Load<Texture2D>("Enemy Sprites\\boofly");
            enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D aspidTexture = Content.Load<Texture2D>("Enemy Sprites\\aspid_hunter");
            Texture2D fireballTexture = Content.Load<Texture2D>("fireball");
            enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D huskBullyTexture = Content.Load<Texture2D>("Enemy Sprites\\husk_bully");
            enemies.Add(new HuskBully(huskBullyTexture, new System.Numerics.Vector2(100, 360)));

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

            // Load tile generators
            levels = new LevelsHandler();
            levels.LoadLevelTiles(Content);


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
            // TODO: Break into physics statics classw

            //    PhysicsHelper.CheckEnemyCollisions(player, enemies, currentBlockIndex);


            if (enemies.Count > 0)
            {
                enemies[currentEnemyIndex].Update(gameTime);
            }
            var handler = new ProjectilePlayerCollisionHandler();
            Rectangle playerBounds = player.GetBounds();

            if (enemies[currentEnemyIndex] is Aspid aspid)
            {
                for (int i = aspid.Projectiles.Count - 1; i >= 0; i--)
                {
                    if (aspid.Projectiles[i].GetBounds().Intersects(playerBounds))
                    {
                        handler.HandleCollision(player, aspid.Projectiles[i]);
                        aspid.Projectiles.RemoveAt(i);
                    }
                }
            }
            var enemyHandler = new PlayerProjectileEnemyCollisionHandler();
            ISprite currentEnemy = enemies[currentEnemyIndex];

            for (int i = player.Projectiles.Count - 1; i >= 0; i--)
            {
                if (currentEnemy is Aspid aspid2 && !aspid2.IsDead)
                {
                    if (player.Projectiles[i].GetBounds().Intersects(aspid2.GetBounds()))
                    {
                        enemyHandler.HandleCollision(aspid2);
                        player.Projectiles.RemoveAt(i);
                    }
                }
                else if (currentEnemy is Boofly boofly && !boofly.IsDead)
                {
                    if (player.Projectiles[i].GetBounds().Intersects(boofly.GetBounds()))
                    {
                        enemyHandler.HandleCollision(boofly);
                        player.Projectiles.RemoveAt(i);
                    }
                }
                else if (currentEnemy is HuskBully huskBully && !huskBully.IsDead)
                {
                    if (player.Projectiles[i].GetBounds().Intersects(huskBully.GetBounds()))
                    {
                        enemyHandler.HandleCollision(huskBully);
                        player.Projectiles.RemoveAt(i);
                    }
                }
            }
            // Melee hitbox vs enemies
            if (player.IsAttacking)
            {
                Rectangle meleeHitbox = player.GetMeleeHitbox();
                if (currentEnemy is Aspid aspidMelee && !aspidMelee.IsDead)
                {
                    if (meleeHitbox.Intersects(aspidMelee.GetBounds()))
                    {
                        aspidMelee.TakeDamage();
                    }
                }
                else if (currentEnemy is Boofly booflyMelee && !booflyMelee.IsDead)
                {
                    if (meleeHitbox.Intersects(booflyMelee.GetBounds()))
                    {
                        booflyMelee.TakeDamage();
                    }
                }
                else if (currentEnemy is HuskBully huskBullyMelee && !huskBullyMelee.IsDead)
                {
                    if (meleeHitbox.Intersects(huskBullyMelee.GetBounds()))
                    {
                        huskBullyMelee.TakeDamage();
                    }
                }
            }
            PhysicsHelper.CheckPlayerGeosCollisions(player, levels.currentGeos, gameTime);


            if (blocks.Count > 0)
            {
                blocks[currentBlockIndex].Update(gameTime);
            }
            player.Update(gameTime);

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


            if (levels.currentTilesGen != null)
            {
                PhysicsHelper.CheckCollisions(player, levels.currentTilesGen);
                PhysicsHelper.CheckEnemyCollisions(player, enemies, currentEnemyIndex, levels.currentTilesGen);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            levels.Draw(_spriteBatch);

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


            foreach (var geo in levels.currentGeos)
            {
                geo.Draw(_spriteBatch);
            }

            player.Draw(_spriteBatch, gameTime);
            itemManager.Draw(_spriteBatch);

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
            instance.levels.CycleStage(direction);
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
            //player.Tiles = drawTilesGen.TileList;


            // Reset enemies
            enemies.Clear();
            Texture2D enemyTexture = Content.Load<Texture2D>("Enemy Sprites\\boofly");
            enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D aspidTexture = Content.Load<Texture2D>("Enemy Sprites\\aspid_hunter");
            enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));
            Texture2D huskBullyTexture = Content.Load<Texture2D>("Enemy Sprites\\husk_bully");
            enemies.Add(new HuskBully(huskBullyTexture, new System.Numerics.Vector2(100, 360)));

            // Reset item manager
            itemManager = new ItemManager(0.4f);
            Texture2D unbreakableHeart = Content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
            Texture2D dashmaster = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
            itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2), new Vector2(10, 10));
            itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

            // Reset geos for both levels
            instance.levels.ClearGeos();

            // Reset indices
            currentEnemyIndex = 0;
            currentBlockIndex = 0;
        }

        // TODO: maybe move this out to its own static helper class
        private Texture2D CreatePixelTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
}
