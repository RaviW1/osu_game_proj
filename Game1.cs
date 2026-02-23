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
        private ItemManager itemManager;
        private List<ISprite> enemies;
        private List<ISprite> blocks;
        private int currentEnemyIndex = 0;
        private int currentBlockIndex = 0;
        private static Game1 instance;
        private AbilityBar abilityBar;
        private Texture2D pixelTexture;
        private SpriteFont font;

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


            keyboard = new KeyboardController();
            keyboard.BindPress(Keys.O, new CycleEnemyCommand(-1));
            keyboard.BindPress(Keys.P, new CycleEnemyCommand(1));


            var moveAxisCmd = new MovementAxisCommand(
                leftKeys: new[] { Keys.A, Keys.Left },
                rightKeys: new[] { Keys.D, Keys.Right }
            );


            var vertAxisCmd = new VerticalAxisCommand(
                upKeys: new[] { Keys.W, Keys.Up },
                downKeys: new[] { Keys.S, Keys.Down }
            );

            var jumpPressedCmd = new JumpPressedCommand();
            var jumpReleasedCmd = new JumpReleasedCommand();

            // ---- Horizontal movement axis (X) ----
            // Update while held
            keyboard.BindHeld(Keys.A, moveAxisCmd);
            keyboard.BindHeld(Keys.Left, moveAxisCmd);
            keyboard.BindHeld(Keys.D, moveAxisCmd);
            keyboard.BindHeld(Keys.Right, moveAxisCmd);

            // Recompute on release (so axis returns to 0 properly)
            keyboard.BindRelease(Keys.A, moveAxisCmd);
            keyboard.BindRelease(Keys.Left, moveAxisCmd);
            keyboard.BindRelease(Keys.D, moveAxisCmd);
            keyboard.BindRelease(Keys.Right, moveAxisCmd);

            // ---- Vertical intent axis (Y) ----
            // Update while held
            keyboard.BindHeld(Keys.W, vertAxisCmd);
            keyboard.BindHeld(Keys.Up, vertAxisCmd);
            keyboard.BindHeld(Keys.S, vertAxisCmd);
            keyboard.BindHeld(Keys.Down, vertAxisCmd);

            // Recompute on release
            keyboard.BindRelease(Keys.W, vertAxisCmd);
            keyboard.BindRelease(Keys.Up, vertAxisCmd);
            keyboard.BindRelease(Keys.S, vertAxisCmd);
            keyboard.BindRelease(Keys.Down, vertAxisCmd);

            // ---- Jump (press + release) ----
            keyboard.BindPress(Keys.Space, jumpPressedCmd);
            keyboard.BindRelease(Keys.Space, jumpReleasedCmd);

            // ---- Item cycling (u = previous, i = next) ----
            itemManager = new ItemManager(0.4f);
            var cyclePrevItemCmd = new CycleItemCommand(-1, (dir) => itemManager.CycleItem(dir, player));
            var cycleNextItemCmd = new CycleItemCommand(1, (dir) => itemManager.CycleItem(dir, player));
            keyboard.BindPress(Keys.U, cyclePrevItemCmd);
            keyboard.BindPress(Keys.I, cycleNextItemCmd);
            keyboard.BindPress(Keys.Q, new QuitCommand(this));
            keyboard.BindPress(Keys.R, new ResetCommand(this));

            // Block cycling (t = previous, y = next)
            keyboard.BindPress(Keys.T, new CycleBlockCommand(-1));
            keyboard.BindPress(Keys.Y, new CycleBlockCommand(1));

            // Attack
            keyboard.BindPress(Keys.Z, new AttackCommand());
            keyboard.BindPress(Keys.N, new AttackCommand());

            // Damage
            keyboard.BindPress(Keys.E, new DamageCommand());
            keyboard.BindPress(Keys.D1, new AttackCommand());
            keyboard.BindPress(Keys.D2, new ShootFireballCommand());
            keyboard.BindHeld(Keys.D3, new HealCommand());



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

            // TODO: use this.Content to load your game content here

            // Load Player Textures
            var playerTextures = new Dictionary<string, Texture2D>();
            playerTextures.Add("Walking", Content.Load<Texture2D>("hollow_knight_walking"));
            playerTextures.Add("Jumping", Content.Load<Texture2D>("knight_jumping"));

            // create new player object
            player = new Player(playerTextures, fireballTexture, new Vector2(350, 200));

            // Load item textures and add to item manager
            // ID 0: Unbreakable Heart (+2 HP on select), ID 1: Dashmaster (canDash on select)
            Texture2D unbreakableHeart = Content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
            Texture2D dashmaster = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
            itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.PlayerHealth += 2, p => p.PlayerHealth -= 2), new Vector2(10, 10));
            itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

            playerTextures.Add("Attack", Content.Load<Texture2D>("hollow_knight_attack"));

            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (enemies.Count > 0)
            {
                enemies[currentEnemyIndex].Update();
            }

            if (blocks.Count > 0)
            {
                blocks[currentBlockIndex].Update();
            }

            List<ICommand> currentCommands = keyboard.GetCommands();

            foreach (ICommand command in currentCommands)
            {
                command.Execute(player);
            }

            itemManager.Update(gameTime);

            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            base.Draw(gameTime);

            // TODO: Add your drawing code here
            if (enemies.Count > 0)
            {
                enemies[currentEnemyIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);
            }

            if (blocks.Count > 0)
            {
                blocks[currentBlockIndex].Draw(_spriteBatch, System.Numerics.Vector2.Zero);
            }
            abilityBar.Draw(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);


            player.Draw(_spriteBatch, gameTime);
            itemManager.Draw(_spriteBatch);

            int viewWidth = GraphicsDevice.Viewport.Width;
            string hpText = "HP " + player.PlayerHealth;
            string dashText = player.CanDash ? "Can Dash" : "Can't Dash";
            Vector2 hpSize = font.MeasureString(hpText);
            Vector2 dashSize = font.MeasureString(dashText);
            float margin = 10f;
            _spriteBatch.DrawString(font, hpText, new Vector2(viewWidth - hpSize.X - margin, margin), Color.White);
            _spriteBatch.DrawString(font, dashText, new Vector2(viewWidth - dashSize.X - margin, margin + hpSize.Y + 4), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
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

        public void Reset()
        {
            // Reset player position
            var playerTextures = new System.Collections.Generic.Dictionary<string, Texture2D>();
            playerTextures.Add("Walking", Content.Load<Texture2D>("hollow_knight_walking"));

            Texture2D fireballTexture = Content.Load<Texture2D>("fireball");

            player = new Player(playerTextures, fireballTexture, new Vector2(350, 200));


            // Reset enemy index
            currentEnemyIndex = 0;

            // Reset block index
            currentBlockIndex = 0;
        }
        private Texture2D CreatePixelTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
}
