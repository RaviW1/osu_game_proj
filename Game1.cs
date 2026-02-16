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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            itemManager = new ItemManager();
            var cyclePrevItemCmd = new CycleItemCommand(-1, (dir) => itemManager.CycleItem(dir));
            var cycleNextItemCmd = new CycleItemCommand(1, (dir) => itemManager.CycleItem(dir));
            keyboard.BindPress(Keys.U, cyclePrevItemCmd);
            keyboard.BindPress(Keys.I, cycleNextItemCmd);

            //           keyboardController = new KeyboardController();

            //j           keyboardController.RegisterCommand(Keys.Right, new WalkCommand(1));
            //         keyboardController.RegisterCommand(Keys.Left, new WalkCommand(-1));


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);



            // TODO: use this.Content to load your game content here

            var playerTextures = new Dictionary<string, Texture2D>();
            playerTextures.Add("Walking", Content.Load<Texture2D>("hollow_knight_walking"));

            player = new Player(playerTextures, new Vector2(350, 200));

            // Load item textures and add to item manager
            Texture2D dashmaster = Content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
            Texture2D dreamshield = Content.Load<Texture2D>("Dreamshield_charm_grimm_markoth_shield");
            itemManager.AddItem(new TextureItem(dashmaster));
            itemManager.AddItem(new TextureItem(dreamshield));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            List<ICommand> currentCommands = keyboard.GetCommands();

            foreach (ICommand command in currentCommands)
            {
                command.Execute(player);
            }

            player.Update(gameTime);
            itemManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            player.Draw(_spriteBatch);
            itemManager.Draw(_spriteBatch, new Vector2(600, 300));

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
