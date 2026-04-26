using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace osu_game_proj
{
    public class MenuScene : IScene
    {
        private readonly GraphicsDevice _graphics;
        private readonly Microsoft.Xna.Framework.Content.ContentManager _content;
        private readonly Game1 _game;

        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;

        private Rectangle _playBtn;
        private Rectangle _quitBtn;
        private Rectangle _achievementsBtn;
        private Rectangle _backBtn;

        private bool _achievementsOpen = false;

        private MouseState _prevMouse;
        private KeyboardState _prevKeyboard;

        private float _alpha = 0f;
        private bool _fadingOut = false;
        private float _fadeOutAlpha = 0f;

        private const float FadeInSpeed = 1.2f;
        private const float FadeOutSpeed = 1.8f;

        public MenuScene(GraphicsDevice graphics,
                         Microsoft.Xna.Framework.Content.ContentManager content,
                         Game1 game)
        {
            _graphics = graphics;
            _content = content;
            _game = game;
        }

        public void Initialize() { }

        public void Load()
        {
            _spriteBatch = new SpriteBatch(_graphics);

            _pixel = new Texture2D(_graphics, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _font = _content.Load<SpriteFont>("DefaultFont");
            _alpha = 0f;
            _fadingOut = false;
            _fadeOutAlpha = 0f;

            _prevMouse = Mouse.GetState();
        }

        public void Unload()
        {
            _pixel?.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_alpha < 1f && !_fadingOut)
                _alpha = MathHelper.Clamp(_alpha + FadeInSpeed * dt, 0f, 1f);

            if (_fadingOut)
            {
                _fadeOutAlpha = MathHelper.Clamp(_fadeOutAlpha + FadeOutSpeed * dt, 0f, 1f);
                if (_fadeOutAlpha >= 1f)
                    LaunchGame();
                return;
            }

            HandleInput();
        }

        public void Draw(SpriteBatch _ignored, GameTime gameTime)
        {
            _graphics.Clear(new Color(10, 8, 16));
            _spriteBatch.Begin();

            if (_achievementsOpen)
                DrawAchievementsScreen();
            else
                DrawMainMenu();

            _spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            int vw = _graphics.Viewport.Width;
            int vh = _graphics.Viewport.Height;

            string title = "Hollow Knight";
            float titleScale = 4f;
            Vector2 titleSize = _font.MeasureString(title) * titleScale;
            _spriteBatch.DrawString(_font, title,
                new Vector2((vw - titleSize.X) / 2f, vh * 0.20f),
                Color.White * _alpha, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

            string sub = "press play to begin";
            Vector2 subSz = _font.MeasureString(sub);
            _spriteBatch.DrawString(_font, sub,
                new Vector2((vw - subSz.X) / 2f, vh * 0.38f),
                Color.Gray * _alpha);

            DrawButton(_spriteBatch, ref _playBtn, "Play", vw / 2, (int)(vh * 0.52f), Color.DarkSlateBlue);
            DrawButton(_spriteBatch, ref _achievementsBtn, "Achievements", vw / 2, (int)(vh * 0.63f), new Color(80, 60, 20));
            DrawButton(_spriteBatch, ref _quitBtn, "Quit", vw / 2, (int)(vh * 0.74f), new Color(80, 30, 30));

            float overlayAlpha = MathHelper.Clamp((1f - _alpha) + _fadeOutAlpha, 0f, 1f);
            if (overlayAlpha > 0f)
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, vw, vh), Color.Black * overlayAlpha);
        }

        private void DrawAchievementsScreen()
        {
            int vw = _graphics.Viewport.Width;
            int vh = _graphics.Viewport.Height;

            // Title
            string title = "Achievements";
            float titleScale = 2.5f;
            Vector2 titleSize = _font.MeasureString(title) * titleScale;
            _spriteBatch.DrawString(_font, title,
                new Vector2((vw - titleSize.X) / 2f, vh * 0.08f),
                Color.Gold, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

            // Achievement rows
            int rowH = 70;
            int padX = 60;
            int startY = (int)(vh * 0.25f);
            int boxW = vw - padX * 2;

            for (int i = 0; i < AchievementManager.All.Length; i++)
            {
                var a = AchievementManager.All[i];
                Rectangle row = new Rectangle(padX, startY + i * (rowH + 12), boxW, rowH);

                Color bgColor = a.Unlocked ? new Color(30, 60, 30) : new Color(30, 30, 30);
                Color textColor = a.Unlocked ? Color.White : Color.Gray;
                Color tagColor = a.Unlocked ? Color.LimeGreen : new Color(80, 80, 80);
                string tag = a.Unlocked ? "UNLOCKED" : "LOCKED";

                _spriteBatch.Draw(_pixel, row, bgColor);

                // Name
                _spriteBatch.DrawString(_font, a.Name,
                    new Vector2(row.X + 16, row.Y + 8), textColor);

                // Description
                float descScale = 0.8f;
                _spriteBatch.DrawString(_font, a.Description,
                    new Vector2(row.X + 16, row.Y + 36), Color.Gray * (a.Unlocked ? 1f : 0.5f),
                    0f, Vector2.Zero, descScale, SpriteEffects.None, 0f);

                Vector2 tagSize = _font.MeasureString(tag);
                _spriteBatch.DrawString(_font, tag,
                    new Vector2(row.Right - tagSize.X - 16, row.Y + (rowH - tagSize.Y) / 2f),
                    tagColor);
            }

            // Back button (upper-left corner)
            Vector2 backSz = _font.MeasureString("Back");
            int backW = (int)backSz.X + 60;
            DrawButton(_spriteBatch, ref _backBtn, "Back", 20 + backW / 2, 30, new Color(50, 50, 80));
        }

        private void HandleInput()
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (ms.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
            {
                if (_achievementsOpen)
                {
                    if (_backBtn.Contains(ms.Position))
                        _achievementsOpen = false;
                }
                else
                {
                    if (_playBtn.Contains(ms.Position)) BeginFadeOut();
                    if (_achievementsBtn.Contains(ms.Position)) _achievementsOpen = true;
                    if (_quitBtn.Contains(ms.Position)) _game.Exit();
                }
            }

            if (!_achievementsOpen)
            {
                if ((ks.IsKeyDown(Keys.Enter) && _prevKeyboard.IsKeyUp(Keys.Enter)) ||
                    (ks.IsKeyDown(Keys.Space) && _prevKeyboard.IsKeyUp(Keys.Space)))
                    BeginFadeOut();
            }
            else
            {
                if (ks.IsKeyDown(Keys.Escape) && _prevKeyboard.IsKeyUp(Keys.Escape))
                    _achievementsOpen = false;
            }

            _prevMouse = ms;
            _prevKeyboard = ks;
        }

        private void DrawButton(SpriteBatch sb, ref Rectangle rect,
                                string label, int cx, int cy, Color bg)
        {
            Vector2 sz = _font.MeasureString(label);
            int bw = (int)sz.X + 60;
            int bh = (int)sz.Y + 24;
            rect = new Rectangle(cx - bw / 2, cy - bh / 2, bw, bh);

            bool hover = rect.Contains(Mouse.GetState().Position);
            sb.Draw(_pixel, rect, (hover ? Color.Lerp(bg, Color.White, 0.15f) : bg) * _alpha);

            sb.DrawString(_font, label,
                new Vector2(rect.X + (rect.Width - sz.X) / 2f, rect.Y + (rect.Height - sz.Y) / 2f),
                Color.White * _alpha);
        }

        private void BeginFadeOut()
        {
            _fadingOut = true;
            _fadeOutAlpha = 0f;
        }

        private void LaunchGame()
        {
            var game = new GameScene(_graphics, _content, _game);
            game.Initialize();
            game.Load();
            _game.SwitchScene(game);
        }
    }
}