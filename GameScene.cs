using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

public class GameScene : IScene
{
    private readonly GraphicsDevice _graphics;
    private readonly ContentManager _content;
    private readonly Game1 _game;

    // Game state
    private Player player;
    private KeyboardController keyboard;
    private MouseController mouse;
    private ItemManager itemManager;
    private List<ISprite> enemies;
    private List<ISprite> blocks;
    private int currentEnemyIndex = 0;
    private int currentBlockIndex = 0;
    private AbilityBar abilityBar;
    private Texture2D pixelTexture;
    private SpriteFont font;
    private Texture2D fireballTexture;

    // Soul Meter HUD
    private Texture2D _soulMeterTexture;
    private Texture2D _hpMaskTexture;

    // Game Over
    private bool _isGameOver;
    private bool _isWin;
    private float _winAlpha;
    private Rectangle _quitButtonRect;
    private float _gameOverAlpha;
    private const float FadeSpeed = 0.8f;
    private Texture2D _gameOverTexture;
    private Rectangle _restartButtonRect;
    private MouseState _previousMouse;
    private bool _isPaused;
    private bool _isTransitioning;
    private float _transitionAlpha;
    private int _pendingTransitionDirection;
    private enum TransitionPhase { FadeOut, FadeIn }
    private TransitionPhase _transitionPhase;
    private const float TransitionSpeed = 2.0f;

    // Rooms
    private LevelsHandler levels;

    // Collision
    private SpatialGrid _grid;

    // Camera
    private Camera2D _camera;

    public GameScene(GraphicsDevice graphics, ContentManager content, Game1 game)
    {
        _graphics = graphics;
        _content = content;
        _game = game;
    }
    public void TogglePause(){
        _isPaused = !_isPaused;
    }

    public void Initialize() { }

    public void Load()
    {
        // Input
        _isWin = false;
        _isPaused = false;
        _winAlpha = 0f;
        keyboard = new KeyboardController();
        BindKeys keyBindObj = new BindKeys(keyboard);
        keyBindObj.bindKeys(this, _game);

        itemManager = new ItemManager(0.4f);
        var cyclePrevItemCmd = new CycleItemCommand(-1, (dir) => itemManager.CycleItem(dir, player));
        var cycleNextItemCmd = new CycleItemCommand(1, (dir) => itemManager.CycleItem(dir, player));
        keyboard.BindPress(Keys.U, cyclePrevItemCmd);
        keyboard.BindPress(Keys.I, cycleNextItemCmd);

        mouse = new MouseController(_game,
            new CycleStageCommand(-1, this),
            new CycleStageCommand(1, this),
            new CycleStageCommand(-1, this),
            new CycleStageCommand(1, this),
            new CycleStageCommand(-1, this));

        levels = new LevelsHandler();
        levels.LoadLevelTiles(_content);
        _grid = new SpatialGrid(64, levels.currentRoom.Tiles);

        pixelTexture = CreatePixelTexture();
        font = _content.Load<SpriteFont>("DefaultFont");

        // UI
        abilityBar = CreateAbilityBar();
        // Blocks
        blocks = CreateBlocks();
        // Player
        player = CreatePlayer();
        // Enemies
        enemies = CreateEnemies();
        // Items
        fireballTexture = _content.Load<Texture2D>("fireball");
        LoadItems();
        // Music
        SoundManager.Initialize(_content);
        SoundManager.PlayBGMusic();

        // Soul Meter & HP Masks
        _soulMeterTexture = _content.Load<Texture2D>("soul_meter");
        StripDarkPixels(_soulMeterTexture, 30);
        _hpMaskTexture = _content.Load<Texture2D>("masks(hp bar)");
        StripDarkPixels(_hpMaskTexture, 30);

        // Game Over
        _gameOverTexture = _content.Load<Texture2D>("Game_Over");
        _isGameOver = false;
        _gameOverAlpha = 0f;

        // Camera — always last
        _camera = new Camera2D(_graphics);
        _camera.RoomBounds = levels.currentRoom.Bounds;
        _camera.SnapTo(player.Position);
    }

    public void Update(GameTime gameTime)
    {
        if (_isPaused){
            ProcessInput(gameTime); 
            return;
        }
        if (_isTransitioning){
            if (_transitionPhase == TransitionPhase.FadeOut){
                _transitionAlpha += TransitionSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_transitionAlpha >= 1f){
                    _transitionAlpha = 1f;
                    levels.CycleStage(_pendingTransitionDirection);
                    _grid = new SpatialGrid(64, levels.currentRoom.Tiles);
                    _camera.RoomBounds = levels.currentRoom.Bounds;
                    _camera.SnapTo(player.Position);
                    _transitionPhase = TransitionPhase.FadeIn;
                }
            }else {
                _transitionAlpha -= TransitionSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_transitionAlpha <= 0f){
                    _transitionAlpha = 0f;
                    _isTransitioning = false;
                }
            }
            return;
        }
        if (_isGameOver)
        {
            if (_gameOverAlpha < 1f)
                _gameOverAlpha = MathHelper.Clamp(_gameOverAlpha + FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f, 1f);

            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released
                && _restartButtonRect.Contains(ms.Position))
            {
                _isGameOver = false;
                _gameOverAlpha = 0f;
                Reset();
            }
            _previousMouse = ms;
            return;
        }
        if (_isWin){
            if (_winAlpha < 1f) _winAlpha = MathHelper.Clamp(_winAlpha + FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f, 1f);

            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released){
                if (_restartButtonRect.Contains(ms.Position)) { _isWin = false; _winAlpha = 0f; Reset(); }
                else if (_quitButtonRect.Contains(ms.Position)) { _game.Exit(); }
            }
            _previousMouse = ms;
            return;
        }

        if (player.PlayerHealth <= 0)
        {
            _isGameOver = true;
            _gameOverAlpha = 0f;
            return;
        }

        Rectangle playerBounds = player.GetBounds();
        ISprite currentEnemy = enemies[currentEnemyIndex];

        // 1. Player collision — sets OnGround
        levels.currentRoom.Update(gameTime, player); // room-specific logic hook
        var playerResults = CollisionSystem.Query(player.GetBounds(), _grid, player.Velocity);
        player.ResolveCollisions(playerResults);

        // 2. Enemy collision
        if (currentEnemy is IEnemy enemyCollision && !enemyCollision.IsDead)
        {
            var enemyVelocity = new Vector2(enemyCollision.GetVelocityX(), enemyCollision.GetVelocityY());
            var enemyResults = CollisionSystem.Query(enemyCollision.GetBounds(), _grid, enemyVelocity);
            enemyCollision.ResolveCollisions(enemyResults);
        }

        // 3. Input next — sets commandReceivedThisFrame before player.Update reads it
        ProcessInput(gameTime);

        // 4. Player update — states now see correct OnGround AND correct input
        player.Update(gameTime);

        // 5. Enemy update
        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Update(gameTime);
        if (blocks.Count > 0)
            blocks[currentBlockIndex].Update(gameTime);

        // 6. Aspid projectiles vs player
        if (currentEnemy is Aspid aspid)
        {
            for (int i = aspid.Projectiles.Count - 1; i >= 0; i--)
            {
                if (aspid.Projectiles[i].GetBounds().Intersects(playerBounds))
                {
                    if (!player.IsInvincible)
                    {
                        player.PlayerHealth--;
                        player.TakeDamage();
                    }
                    aspid.Projectiles.RemoveAt(i);
                }
            }
        }

        // 7. Player projectiles vs enemy
        if (currentEnemy is IEnemy targetEnemy && !targetEnemy.IsDead)
        {
            for (int i = player.Projectiles.Count - 1; i >= 0; i--)
            {
                if (player.Projectiles[i].GetBounds().Intersects(targetEnemy.GetBounds()))
                {
                    targetEnemy.TakeDamage();
                    player.Projectiles.RemoveAt(i);
                }
            }
        }

        // 8. Melee vs enemy
        if (player.IsAttacking && currentEnemy is IEnemy meleeTarget && !meleeTarget.IsDead)
        {
            if (player.GetMeleeHitbox().Intersects(meleeTarget.GetBounds()))
                meleeTarget.TakeDamage();
        }

        // 9. Geo collection
        for (int i = levels.currentGeos.Count - 1; i >= 0; i--)
        {
            if (!levels.currentGeos[i].IsCollected && levels.currentGeos[i].GetBounds().Intersects(playerBounds))
            {
                levels.currentGeos[i].Collect();
                player.GeoCount++;
            }
            levels.currentGeos[i].Update(gameTime);
        }

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Update(gameTime);

        itemManager.Update(gameTime);

        _camera.Follow(player.Position);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Pass 1 — world space
        spriteBatch.Begin(transformMatrix: _camera.GetTransform());
        levels.Draw(spriteBatch);

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);

        foreach (var geo in levels.currentGeos)
            geo.Draw(spriteBatch);

        player.Draw(spriteBatch, gameTime);

        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);
        spriteBatch.End();

        // Pass 2 — soul meter & HP masks (non-premultiplied textures)
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        DrawSoulMeter(spriteBatch);
        DrawHPBar(spriteBatch);
        spriteBatch.End();

        // Pass 3 — remaining HUD
        spriteBatch.Begin();
        if (_isPaused) DrawPauseScreen(spriteBatch);
        abilityBar.Draw(spriteBatch, _graphics.Viewport.Width, _graphics.Viewport.Height);
        HUD.DrawHUD(player, spriteBatch, _graphics.Viewport.Width, font, levels.geoTexture);

        if (_isGameOver)
            DrawGameOver(spriteBatch);
        if (_isWin)
            DrawWinScreen(spriteBatch);
        
        if (_isTransitioning) spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, _graphics.Viewport.Width, _graphics.Viewport.Height), Color.Black * _transitionAlpha);

        spriteBatch.End();
    }

    public void Unload() { }

    public void CycleEnemy(int direction)
    {
        if (enemies.Count == 0) return;
        currentEnemyIndex += direction;
        if (currentEnemyIndex < 0)
            currentEnemyIndex = enemies.Count - 1;
        else if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = 0;
    }

    public void CycleBlock(int direction)
    {
        if (blocks.Count == 0) return;
        currentBlockIndex += direction;
        if (currentBlockIndex < 0)
            currentBlockIndex = blocks.Count - 1;
        else if (currentBlockIndex >= blocks.Count)
            currentBlockIndex = 0;
    }
    public void TriggerWin(){
        _isWin = true;
        _winAlpha = 0f;
    }

    public void CycleStage(int direction)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        _transitionAlpha = 0f;
        _transitionPhase = TransitionPhase.FadeOut;
        _pendingTransitionDirection = direction;
    }

    public void Reset()
    {
        enemies = CreateEnemies();
        player = CreatePlayer();

        itemManager = new ItemManager(0.4f);
        LoadItems();

        currentEnemyIndex = 0;
        currentBlockIndex = 0;

        _grid = new SpatialGrid(64, levels.currentRoom.Tiles);

        levels.ClearGeos();
        _camera.SnapTo(player.Position);
    }

    private void ProcessInput(GameTime gameTime)
    {
        foreach (ICommand cmd in keyboard.GetCommands(gameTime))
            cmd.Execute(player, gameTime);
        foreach (ICommand cmd in mouse.GetCommands(gameTime))
            cmd.Execute(player, gameTime);
    }

    private List<ISprite> CreateEnemies()
    {
        Texture2D booflyTex = _content.Load<Texture2D>("Enemy Sprites\\boofly");
        Texture2D aspidTex = _content.Load<Texture2D>("Enemy Sprites\\aspid_hunter");
        Texture2D huskBullyTex = _content.Load<Texture2D>("Enemy Sprites\\husk_bully");

        return new List<ISprite>
        {
            new Boofly(booflyTex, new System.Numerics.Vector2(500, 50)),
            new Aspid(aspidTex, fireballTexture, new System.Numerics.Vector2(500, 50)),
            new HuskBully(huskBullyTex, new System.Numerics.Vector2(100, 360))
        };
    }

    private List<ISprite> CreateBlocks()
    {
        Texture2D spikeTex = _content.Load<Texture2D>("spike_back");
        Texture2D fungalSpikeTex = _content.Load<Texture2D>("fungd_spikes_01");

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
            { "Walking", _content.Load<Texture2D>("hollow_knight_walking") },
            { "Jumping", _content.Load<Texture2D>("knight_jumping") },
            { "Attacking", _content.Load<Texture2D>("knight_attack") },
            { "Attack", _content.Load<Texture2D>("hollow_knight_attack") }
        };
        return new Player(textures, fireballTexture, new Vector2(350, 370));
    }

    private AbilityBar CreateAbilityBar()
    {
        Texture2D playerTex = _content.Load<Texture2D>("hollow_knight_walking");
        fireballTexture = _content.Load<Texture2D>("fireball");

        var icons = new Dictionary<string, Texture2D>
        {
            { "Attack", _content.Load<Texture2D>("hollow_knight_attack") },
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

    private void LoadItems()
    {
        Texture2D heartTex = _content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
        Texture2D dashTex = _content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");

        itemManager.AddItem(
            new TextureItem(0, heartTex, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2),
            new Vector2(10, 10));
        itemManager.AddItem(
            new TextureItem(1, dashTex, p => p.CanDash = true, p => p.CanDash = false),
            new Vector2(100, 10));
    }

    private void DrawGameOver(SpriteBatch spriteBatch)
    {
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;
        Color tint = Color.White * _gameOverAlpha;

        spriteBatch.Draw(_gameOverTexture, new Rectangle(0, 0, vw, vh), tint);

        string title = "Game Over";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        Vector2 titlePos = new Vector2((vw - titleSize.X) / 2f, vh * 0.3f);
        spriteBatch.DrawString(font, title, titlePos, Color.White * _gameOverAlpha,
            0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string btnText = "Restart";
        Vector2 btnTextSize = font.MeasureString(btnText);
        int btnW = (int)btnTextSize.X + 40;
        int btnH = (int)btnTextSize.Y + 20;
        _restartButtonRect = new Rectangle((vw - btnW) / 2, (int)(vh * 0.5f), btnW, btnH);

        spriteBatch.Draw(pixelTexture, _restartButtonRect, Color.DarkGray * _gameOverAlpha);
        Vector2 btnTextPos = new Vector2(
            _restartButtonRect.X + (_restartButtonRect.Width - btnTextSize.X) / 2f,
            _restartButtonRect.Y + (_restartButtonRect.Height - btnTextSize.Y) / 2f);
        spriteBatch.DrawString(font, btnText, btnTextPos, Color.White * _gameOverAlpha);
    }

    private void DrawSoulMeter(SpriteBatch spriteBatch)
    {
        int cellW = _soulMeterTexture.Width / 2;
        int cellH = _soulMeterTexture.Height / 3;
        int yOffset = cellH - 15;
        int drawW = cellW * 2 / 3;
        int drawH = cellH * 2 / 3;
        Rectangle sourceRect = new Rectangle(0, yOffset, cellW, cellH);
        Rectangle destRect = new Rectangle(10, 10, drawW, drawH);
        spriteBatch.Draw(_soulMeterTexture, destRect, sourceRect, Color.White);
    }

    private void DrawHPBar(SpriteBatch spriteBatch)
    {
        int framW = _hpMaskTexture.Width / 14;
        int framH = _hpMaskTexture.Height / 14;
        int pad = 6;

        Rectangle fullSrc = new Rectangle(pad, pad + 3, framW - pad * 2, framH - pad);
        Rectangle emptySrc = new Rectangle(pad, framH * 2 + pad + 8, framW - pad * 2, framH - pad);

        int soulDrawW = (_soulMeterTexture.Width / 2) * 2 / 3;
        int startX = 10 + soulDrawW / 2;
        int startY = 10 + (_soulMeterTexture.Height / 3) * 2 / 3 * 2 / 3;
        int drawSize = 30;
        int spacing = 2;

        for (int i = 0; i < player.MaxPlayerHealth; i++)
        {
            Rectangle dest = new Rectangle(startX + i * (drawSize + spacing), startY, drawSize, drawSize);
            if (i < player.PlayerHealth)
                spriteBatch.Draw(_hpMaskTexture, dest, fullSrc, Color.White);
            else
                spriteBatch.Draw(_hpMaskTexture, dest, emptySrc, Color.White);
        }
    }

    private static void StripDarkPixels(Texture2D texture, int threshold)
    {
        Color[] pixels = new Color[texture.Width * texture.Height];
        texture.GetData(pixels);
        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];
            if (c.R <= threshold && c.G <= threshold && c.B <= threshold)
                pixels[i] = Color.Transparent;
        }
        texture.SetData(pixels);
    }

    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(_graphics, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    private void DrawWinScreen(SpriteBatch spriteBatch){
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.75f * _winAlpha);

        string title = "You Win!";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(font, title, new Vector2((vw - titleSize.X) / 2f, vh * 0.25f),
        Color.Gold * _winAlpha, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string replayText = "Replay";
        Vector2 replaySize = font.MeasureString(replayText);
        int btnW = (int)replaySize.X + 40, btnH = (int)replaySize.Y + 20;
        _restartButtonRect = new Rectangle((vw / 2) - btnW - 20, (int)(vh * 0.5f), btnW, btnH);
        spriteBatch.Draw(pixelTexture, _restartButtonRect, Color.DarkGreen * _winAlpha);
        spriteBatch.DrawString(font, replayText, new Vector2(_restartButtonRect.X + 20, _restartButtonRect.Y + 10), Color.White * _winAlpha);

        string quitText = "Quit";
        Vector2 quitSize = font.MeasureString(quitText);
        int quitW = (int)quitSize.X + 40, quitH = (int)quitSize.Y + 20;
        _quitButtonRect = new Rectangle((vw / 2) + 20, (int)(vh * 0.5f), quitW, quitH);
        spriteBatch.Draw(pixelTexture, _quitButtonRect, Color.DarkRed * _winAlpha);
        spriteBatch.DrawString(font, quitText, new Vector2(_quitButtonRect.X + 20, _quitButtonRect.Y + 10), Color.White * _winAlpha);
    }
    private void DrawPauseScreen(SpriteBatch spriteBatch){
        int vw = _graphics.Viewport.Width;
        int vh = _graphics.Viewport.Height;

        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, vw, vh), Color.Black * 0.5f);

        string title = "Paused";
        float titleScale = 2.5f;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(font, title, new Vector2((vw - titleSize.X) / 2f, vh * 0.25f),
            Color.White, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        string resumeText = "Press ESC to Resume";
        Vector2 resumeSize = font.MeasureString(resumeText);
        spriteBatch.DrawString(font, resumeText, new Vector2((vw - resumeSize.X) / 2f, vh * 0.5f), Color.LightGray);
    }
}
