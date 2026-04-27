using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Collections.Generic;
using System.Linq;

public partial class GameScene : IScene
{
    // Core references
    private readonly GraphicsDevice _graphics;
    private readonly ContentManager _content;
    private readonly Game1 _game;

    // Gameplay objects
    private Player player;
    private KeyboardController keyboard;
    private MouseController mouse;
    private ItemManager itemManager;
    private List<ISprite> blocks;
    private int currentBlockIndex = 0;
    private Texture2D fireballTexture;
    private AbilityBar abilityBar;

    // Level, collision, camera
    private LevelsHandler levels;
    private SpatialGrid _grid;
    private Camera2D _camera;

    // HUD textures
    private Texture2D pixelTexture;
    private SpriteFont font;
    private Texture2D _soulMeterTexture;
    private Texture2D _hpMaskTexture;
    private Texture2D _gameOverTexture;

    // UI state
    private bool _isPaused;
    private bool _isGameOver;
    private bool _isTrapped;
    private bool _isWin;
    private bool _charmInventoryOpen;
    private bool _isTransitioning;
    private bool _isShopOpen;
    private bool _tookHit = false;
    private Rectangle _shopBuyButtonRect;
    private Rectangle _shopHealButtonRect;
    private float _gameOverAlpha;
    private float _winAlpha;
    private float _transitionAlpha;
    private int _pendingTransitionDirection;
    private const float FadeSpeed = 0.8f;
    private const float TransitionSpeed = 2.0f;

    // Boss-defeat win sequence (shake -> fade to white -> win screen)
    private bool _isBossWinSequence;
    private float _bossWinTimer;
    private float _bossWhiteAlpha;
    private const float BossShakeDuration = 1.5f;
    private const float BossFadeInDuration = 1.0f;
    private const float BossHoldDuration = 0.5f;
    private const float BossSequenceTotal = BossShakeDuration + BossFadeInDuration + BossHoldDuration;

    // Secret-boss (Elder Baldur) defeat sequence (shake -> warp up to shop2)
    private bool _isSecretBossSequence;
    private float _secretBossTimer;
    private const float SecretBossShakeDuration = 2.0f;
    private enum TransitionPhase { FadeOut, FadeIn }
    private TransitionPhase _transitionPhase;
    private Rectangle _restartButtonRect;
    private Rectangle _quitButtonRect;
    private Rectangle _mainMenuButtonRect;
    private MouseState _previousMouse;
    private KeyboardState _prevKeyboard;
    private bool _puzzleSolved = false;
    private List<PushBox> _pushBoxes = new List<PushBox>();
    private Vector2[] _savedPushBoxPositions = null;

    public GameScene(GraphicsDevice graphics, ContentManager content, Game1 game)
    {
        _graphics = graphics;
        _content = content;
        _game = game;
    }

    public void Initialize() { }

    public void Load()
    {
        _puzzleSolved = false;
        _isWin = false;
        _savedPushBoxPositions = null;
        _pushBoxes.Clear();
        _isPaused = false;
        _winAlpha = 0f;
        _isBossWinSequence = false;
        _bossWinTimer = 0f;
        _bossWhiteAlpha = 0f;
        _isSecretBossSequence = false;
        _secretBossTimer = 0f;

        keyboard = new KeyboardController();
        new BindKeys(keyboard).bindKeys(this, _game);

        itemManager = new ItemManager(0.4f);
        mouse = new MouseController(_game,
            new CycleStageCommand(1, this),
            new CycleStageCommand(1, this),
            new CycleStageCommand(-1, this),
            new CycleStageCommand(1, this),
            new CycleStageCommand(-1, this));

        levels = new LevelsHandler();
        levels.LoadLevelTiles(_content);
        WireEnemyCallbacks();
        _grid = new SpatialGrid(64, levels.currentRoom.Tiles);

        pixelTexture = CreatePixelTexture();
        font = _content.Load<SpriteFont>("DefaultFont");

        abilityBar = CreateAbilityBar();
        blocks = CreateBlocks();
        player = CreatePlayer();
        fireballTexture = _content.Load<Texture2D>("fireball");
        LoadItems();

        SoundManager.Initialize(_content);
        SoundManager.PlayBGMusic();

        _soulMeterTexture = _content.Load<Texture2D>("soul_meter");
        StripDarkPixels(_soulMeterTexture, 30);
        _hpMaskTexture = _content.Load<Texture2D>("masks(hp bar)");
        StripDarkPixels(_hpMaskTexture, 30);

        _gameOverTexture = _content.Load<Texture2D>("Game_Over");
        _isGameOver = false;
        _isTrapped = false;
        _gameOverAlpha = 0f;

        _camera = new Camera2D(_graphics);
        _camera.RoomBounds = levels.currentRoom.Bounds;
        _camera.SnapTo(player.Position);
        _tookHit = false;
        _pushBoxes.Clear();
        if (levels.currentRoom.roomName == "shop2")
            SpawnPushBoxes();

        LoadFogEffect();  
        UpdateFog();      
    }

    public void Unload() { }

    // ------------------------------------------------------------------
    //  Update
    // ------------------------------------------------------------------

    public void Update(GameTime gameTime)
    {
        if (_isPaused)
        {
            ProcessInput(gameTime);

            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            {
                if (_mainMenuButtonRect.Contains(ms.Position))
                {
                    var menu = new MenuScene(_graphics, _content, _game);
                    menu.Initialize();
                    menu.Load();
                    _game.SwitchScene(menu);
                }
            }
            _previousMouse = ms;
            return;
        }

        if (_isTransitioning) { UpdateTransition(gameTime); return; }
        if (_isGameOver) { UpdateGameOver(gameTime); return; }
        if (_isBossWinSequence) { UpdateBossWinSequence(gameTime); return; }
        if (_isSecretBossSequence) { UpdateSecretBossSequence(gameTime); return; }
        if (_isWin) { UpdateWin(gameTime); return; }
        if (UpdateShop()) return;
        if (UpdateCharmInventory(gameTime)) return;

        if (player.PlayerHealth <= 0)
        {
            _isGameOver = true;
            _gameOverAlpha = 0f;
            return;
        }

        UpdateGameplay(gameTime);
    }

    private void UpdateTransition(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_transitionPhase == TransitionPhase.FadeOut)
        {
            _transitionAlpha += TransitionSpeed * dt;
            if (_transitionAlpha >= 1f)
            {
                _transitionAlpha = 1f;

                if (_pushBoxes.Count > 0)
                    _savedPushBoxPositions = _pushBoxes.Select(b => b.Position).ToArray();

                _pushBoxes.Clear();

                levels.CycleStage(_pendingTransitionDirection);
                if (levels.currentRoom.roomName == "secret")
                    AchievementManager.Unlock(AchievementManager.WhatsThis);
                WireEnemyCallbacks();
                _grid = new SpatialGrid(64, levels.currentRoom.Tiles);
                _camera.RoomBounds = levels.currentRoom.Bounds;

                if (_pendingTransitionDirection == 1)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromLeft");
                else if (_pendingTransitionDirection == -1)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromRight");
                else if (_pendingTransitionDirection == 2)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromUnder");
                else if (_pendingTransitionDirection == -2)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromUp");

                player.Velocity = Vector2.Zero;
                _camera.SnapTo(player.Position);

                if (levels.currentRoom.roomName == "shop2")
                {
                    SpawnPushBoxes();
                    if (_savedPushBoxPositions != null)
                        for (int i = 0; i < _pushBoxes.Count; i++)
                            _pushBoxes[i].Position = _savedPushBoxPositions[i];
                }



                _transitionPhase = TransitionPhase.FadeIn;
            }
        }
        else
        {
            _transitionAlpha -= TransitionSpeed * dt;
            if (_transitionAlpha <= 0f)
            {
                _transitionAlpha = 0f;
                _isTransitioning = false;
            }
        }
        UpdateFog();
    }

    private void UpdateGameOver(GameTime gameTime)
    {
        if (_gameOverAlpha < 1f)
            _gameOverAlpha = MathHelper.Clamp(
                _gameOverAlpha + FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f, 1f);

        MouseState ms = Mouse.GetState();
        if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released
            && _restartButtonRect.Contains(ms.Position))
        {
            _isGameOver = false;
            _isTrapped = false;
            _gameOverAlpha = 0f;
            Reset();
        }
        _previousMouse = ms;
    }

    private void UpdateBossWinSequence(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _bossWinTimer += dt;

        if (_bossWinTimer < BossShakeDuration + BossFadeInDuration)
        {
            _camera.Shake(8f, 6);

            if (_bossWinTimer >= BossShakeDuration)
            {
                float t = (_bossWinTimer - BossShakeDuration) / BossFadeInDuration;
                _bossWhiteAlpha = MathHelper.Clamp(t, 0f, 1f);
            }
        }
        else if (_bossWinTimer < BossSequenceTotal)
        {
            _bossWhiteAlpha = 1f;
        }
        else
        {
            _bossWhiteAlpha = 1f;
            _isBossWinSequence = false;
            TriggerWin();
        }

        UpdateGameplay(gameTime);
    }

    private void UpdateSecretBossSequence(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _secretBossTimer += dt;

        _camera.Shake(8f, 6);

        if (_secretBossTimer >= SecretBossShakeDuration)
        {
            _isSecretBossSequence = false;
            _secretBossTimer = 0f;
            CycleStage(2);
            return;
        }

        UpdateGameplay(gameTime);
    }

    private void UpdateWin(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_winAlpha < 1f)
            _winAlpha = MathHelper.Clamp(_winAlpha + FadeSpeed * dt, 0f, 1f);
        if (_bossWhiteAlpha > 0f)
            _bossWhiteAlpha = MathHelper.Clamp(_bossWhiteAlpha - FadeSpeed * dt, 0f, 1f);

        MouseState ms = Mouse.GetState();
        if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
            if (_restartButtonRect.Contains(ms.Position))
            {
                _isWin = false;
                _winAlpha = 0f;
                Reset();
            }
            else if (_mainMenuButtonRect.Contains(ms.Position))
            {
                var menu = new MenuScene(_graphics, _content, _game);
                menu.Initialize();
                menu.Load();
                _game.SwitchScene(menu);
            }
            else if (_quitButtonRect.Contains(ms.Position))
            {
                _game.Exit();
            }
        }
        _previousMouse = ms;
    }

    private bool UpdateCharmInventory(GameTime gameTime)
    {
        KeyboardState ks = Keyboard.GetState();
        if (ks.IsKeyDown(Keys.I) && _prevKeyboard.IsKeyUp(Keys.I) && !_isShopOpen)
            _charmInventoryOpen = !_charmInventoryOpen;
        _prevKeyboard = ks;

        if (!_charmInventoryOpen) return false;

        if (_charmDenyTimer > 0f)
            _charmDenyTimer = System.Math.Max(0f, _charmDenyTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);

        MouseState ms = Mouse.GetState();
        if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            HandleCharmClick(ms.Position);
        _previousMouse = ms;
        return true;
    }

    private bool UpdateShop()
    {
        KeyboardState ks = Keyboard.GetState();
        bool inShopRoom = levels.currentRoom.roomName == "shop"
                       || levels.currentRoom.roomName == "shop2";

        if (!inShopRoom) _isShopOpen = false;

        if (ks.IsKeyDown(Keys.B) && _prevKeyboard.IsKeyUp(Keys.B) && inShopRoom && !_charmInventoryOpen)
            _isShopOpen = !_isShopOpen;

        if (!_isShopOpen) return false;

        _prevKeyboard = ks;

        MouseState ms = Mouse.GetState();
        if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            HandleShopClick(ms.Position);
        _previousMouse = ms;
        return true;
    }

    private void UpdateGameplay(GameTime gameTime)
    {
        _hitstop.Update();
        if (_hitstop.IsActive) return;

        Rectangle playerBounds = player.GetBounds();

        levels.currentRoom.Update(gameTime, player, this);

        ProcessInput(gameTime);
        player.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        player.ApplyPhysics(dt);

        if (player.WallContact == 0 ||
            (player.WallContact == 1 && player.Velocity.X <= 0) ||
            (player.WallContact == -1 && player.Velocity.X >= 0))
        {
            player.Position.X += player.Velocity.X * dt;
        }

        var playerResults = CollisionSystem.Query(player.GetBounds(), _grid, player.Velocity);
        player.ResolveCollisions(playerResults);

        levels.Update(gameTime, player, _grid);
        SpawnDeathGeos();

        for (int i = levels.currentGeos.Count - 1; i >= 0; i--)
        {
            Geo geo = levels.currentGeos[i];
            geo.Update(gameTime);

            if (geo.IsFalling)
            {
                foreach (TileBlock tile in levels.currentRoom.Tiles)
                {
                    if (tile.isCollideable && geo.GetBounds().Intersects(tile.bounds))
                    {
                        geo.Land();
                        break;
                    }
                }
            }

            if (!geo.IsCollected && geo.GetBounds().Intersects(playerBounds))
            {
                geo.Collect();
                player.GeoCount++;
                if (player.GeoCount >= 100)
                    AchievementManager.Unlock(AchievementManager.Monopoly);
            }
        }

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Update(gameTime);

        itemManager.Update(gameTime);
        UpdatePushBoxes(gameTime);
        _camera.Follow(player.Position);

        if (!_isTrapped && IsTrappedAgainstSecretBoss())
        {
            _isTrapped = true;
            _isGameOver = true;
            _gameOverAlpha = 0f;
        }
    }

    private bool IsTrappedAgainstSecretBoss()
    {
        if (levels.currentRoom.roomName != "secret") return false;
        if (player.Soul >= 10) return false;
        if (_isSecretBossSequence || _isBossWinSequence) return false;

        bool baldurAlive = false;
        bool otherAlive = false;
        foreach (var e in levels.currentEnemyGen.Enemies)
        {
            if (e is BaldurBoss b)
            {
                if (!b.IsDead) baldurAlive = true;
            }
            else if (!e.IsDead)
            {
                otherAlive = true;
            }
        }
        return baldurAlive && !otherAlive;
    }

    // ------------------------------------------------------------------
    //  Draw
    // ------------------------------------------------------------------

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        DrawWorld(spriteBatch, gameTime);
        DrawMasks(spriteBatch);
        DrawHUD(spriteBatch);
    }

    // ------------------------------------------------------------------ 
    //  Pass 1: World (tiles, enemies, player, geos)
    //  If fog is enabled, render to a RenderTarget first then apply shader
    // ------------------------------------------------------------------
    private void DrawWorld(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Draw world normally
        spriteBatch.Begin(transformMatrix: _camera.GetTransform());
        levels.Draw(spriteBatch);
        foreach (var geo in levels.currentGeos)
            geo.Draw(spriteBatch);
        player.Draw(spriteBatch, gameTime);
        foreach (var box in _pushBoxes)
            box.Draw(spriteBatch);
        levels.DrawEnemies(spriteBatch);
        spriteBatch.End();

        // Draw fog vignette overlay in screen space on top
        if (_fogEnabled)
            DrawFogOverlay(spriteBatch);
    }

    private void DrawFogOverlay(SpriteBatch spriteBatch)
    {
        int w = _graphics.Viewport.Width;
        int h = _graphics.Viewport.Height;

        Vector2 screenPos = new Vector2(
            player.Position.X - _camera.Position.X,
            player.Position.Y - _camera.Position.Y);

        spriteBatch.Begin(blendState: BlendState.AlphaBlend);

        int tileSize = 4; // very small = smooth looking
        float innerRadius = 150f;
        float outerRadius = 320f;

        for (int x = 0; x < w; x += tileSize)
        {
            for (int y = 0; y < h; y += tileSize)
            {
                Vector2 tileCenter = new Vector2(x + tileSize / 2f, y + tileSize / 2f);
                float dist = Vector2.Distance(tileCenter, screenPos);

                float alpha = MathHelper.Clamp(
                    (dist - innerRadius) / (outerRadius - innerRadius),
                    0f, 1f);
                // smoothstep
                alpha = alpha * alpha * (3f - 2f * alpha);

                if (alpha > 0.01f)
                {
                    spriteBatch.Draw(pixelTexture,
                        new Rectangle(x, y, tileSize, tileSize),
                        Color.Black * alpha);
                }
            }
        }

        spriteBatch.End();
    }

    // ------------------------------------------------------------------
    //  Pass 2: Soul meter and HP bar (non-premultiplied blend)
    // ------------------------------------------------------------------
    private void DrawMasks(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        DrawSoulMeter(spriteBatch);
        DrawHPBar(spriteBatch);
        spriteBatch.End();
    }

    // ------------------------------------------------------------------
    //  Pass 3: HUD and overlays (always on top, never fogged)
    // ------------------------------------------------------------------
    private void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        if (_isPaused)
            DrawPauseScreen(spriteBatch);

        abilityBar.Draw(spriteBatch, _graphics.Viewport.Width, _graphics.Viewport.Height);
        HUD.DrawHUD(player, spriteBatch, _graphics.Viewport.Width, font, levels.geoTexture);

        if (_isGameOver)
            DrawGameOver(spriteBatch);

        if (_bossWhiteAlpha > 0f)
            DrawFullscreenOverlay(spriteBatch, Color.White * _bossWhiteAlpha);

        if (_isWin)
            DrawWinScreen(spriteBatch);

        if (_charmInventoryOpen)
            DrawCharmInventory(spriteBatch);

        if (_isShopOpen)
            DrawShopHUD(spriteBatch);

        if (_isTransitioning)
            DrawFullscreenOverlay(spriteBatch, Color.Black * _transitionAlpha);

        if (itemManager.IsEquipped(WaywardCompassIndex))
            Minimap.Draw(spriteBatch, pixelTexture, _graphics,
                levels.currentRoom.Bounds, levels.currentRoom.Tiles,
                player.Position, levels.TotalRooms, levels.CurrentRoomIndex);

        DrawRoomHints(spriteBatch);

        spriteBatch.End();
    }

    // ------------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------------
    private void DrawFullscreenOverlay(SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(pixelTexture,
            new Rectangle(0, 0, _graphics.Viewport.Width, _graphics.Viewport.Height),
            color);
    }

    private void DrawRoomHints(SpriteBatch spriteBatch)
    {
        if (_isGameOver || _isPaused || _charmInventoryOpen) return;

        bool inShop = levels.currentRoom.roomName == "shop"
                   || levels.currentRoom.roomName == "shop2";

        if (inShop && !_isShopOpen)
        {
            DrawCenteredHint(spriteBatch, "Press B to open Shop");
            return;
        }

        if (levels.currentRoom.roomName == "level1" && !_isShopOpen)
        {
            DrawCenteredHint(spriteBatch, "Press I to open Inventory");
        }
    }

    private void DrawCenteredHint(SpriteBatch spriteBatch, string text)
    {
        Vector2 size = font.MeasureString(text);
        Vector2 pos = new Vector2((_graphics.Viewport.Width - size.X) / 2f, 20);
        spriteBatch.DrawString(font, text, pos, Color.Gold);
    }

    // ------------------------------------------------------------------
    //  Public actions
    // ------------------------------------------------------------------

    public void TogglePause() => _isPaused = !_isPaused;

    public void TriggerWin()
    {
        _isWin = true;
        _winAlpha = 0f;
        AchievementManager.Unlock(AchievementManager.Winner);
        if (!_tookHit)
            AchievementManager.Unlock(AchievementManager.Robinhood);
    }

    public void CycleStage(int direction)
    {
        if (direction == 1)
            player.Position = levels.currentRoom.GetSpawnPoint("fromLeft");
        else if (direction == -1)
            player.Position = levels.currentRoom.GetSpawnPoint("fromRight");

        if (_isTransitioning) return;
        _isTransitioning = true;
        _transitionAlpha = 0f;
        _transitionPhase = TransitionPhase.FadeOut;
        _pendingTransitionDirection = direction;
    }

    public void Reset()
    {
        _puzzleSolved = false;
        _savedPushBoxPositions = null;
        _pushBoxes.Clear();
        _charmInventoryOpen = false;
        _isShopOpen = false;
        _isBossWinSequence = false;
        _bossWinTimer = 0f;
        _bossWhiteAlpha = 0f;
        _isSecretBossSequence = false;
        _secretBossTimer = 0f;
        _isTrapped = false;

        levels.ResetToFirstLevel();
        levels.ResetAllEnemies();
        levels.ClearGeos();

        player = CreatePlayer();
        WireEnemyCallbacks();

        itemManager = new ItemManager(0.4f);
        LoadItems();
        currentBlockIndex = 0;

        _grid = new SpatialGrid(64, levels.currentRoom.Tiles);
        _camera.RoomBounds = levels.currentRoom.Bounds;
        _camera.SnapTo(player.Position);
        _tookHit = false;


    }

    // ------------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------------

    private void WireEnemyCallbacks()
    {
        levels.currentEnemyGen.OnPlayerHit = () => TriggerHitEffects(playerWasHit: true);
        levels.currentEnemyGen.OnEnemyHit = () => TriggerHitEffects(playerWasHit: false);
        levels.currentEnemyGen.OnBossDeath = () => BeginBossWinSequence();
        levels.currentEnemyGen.OnSecretBossDeath = () => BeginSecretBossSequence();
    }

    public void BeginBossWinSequence()
    {
        if (_isBossWinSequence || _isWin) return;
        _isBossWinSequence = true;
        _bossWinTimer = 0f;
        _bossWhiteAlpha = 0f;
    }

    public void BeginSecretBossSequence()
    {
        if (_isSecretBossSequence || _isTransitioning) return;
        _isSecretBossSequence = true;
        _secretBossTimer = 0f;
    }
}