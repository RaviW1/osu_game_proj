using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Collections.Generic;

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
    private List<ISprite> enemies;
    private List<ISprite> blocks;
    private int currentEnemyIndex = 0;
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
    private bool _isWin;
    private bool _charmInventoryOpen;
    private bool _isTransitioning;
    private bool _isShopOpen;
    private bool _tookHit = false;
    private Rectangle _shopBuyButtonRect;
    private float _gameOverAlpha;
    private float _winAlpha;
    private float _transitionAlpha;
    private int _pendingTransitionDirection;
    private const float FadeSpeed = 0.8f;
    private const float TransitionSpeed = 2.0f;
    private enum TransitionPhase { FadeOut, FadeIn }
    private TransitionPhase _transitionPhase;
    private Rectangle _restartButtonRect;
    private Rectangle _quitButtonRect;
    private Rectangle _mainMenuButtonRect;
    private MouseState _previousMouse;
    private KeyboardState _prevKeyboard;

    public GameScene(GraphicsDevice graphics, ContentManager content, Game1 game)
    {
        _graphics = graphics;
        _content = content;
        _game = game;
    }

    public void Initialize() { }

    public void Load()
    {
        _isWin = false;
        _isPaused = false;
        _winAlpha = 0f;

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
        itemManager.VisibleCount = GetVisibleCharmCount();
        itemManager.EquipItem(0, player);

        SoundManager.Initialize(_content);
        SoundManager.PlayBGMusic();

        _soulMeterTexture = _content.Load<Texture2D>("soul_meter");
        StripDarkPixels(_soulMeterTexture, 30);
        _hpMaskTexture = _content.Load<Texture2D>("masks(hp bar)");
        StripDarkPixels(_hpMaskTexture, 30);

        _gameOverTexture = _content.Load<Texture2D>("Game_Over");
        _isGameOver = false;
        _gameOverAlpha = 0f;

        _camera = new Camera2D(_graphics);
        _camera.RoomBounds = levels.currentRoom.Bounds;
        _camera.SnapTo(player.Position);
        _tookHit = false;
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
        if (_isWin) { UpdateWin(gameTime); return; }
        if (UpdateShop()) return;
        if (UpdateCharmInventory()) return;

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
                levels.CycleStage(_pendingTransitionDirection);
                WireEnemyCallbacks();
                _grid = new SpatialGrid(64, levels.currentRoom.Tiles);
                _camera.RoomBounds = levels.currentRoom.Bounds;

                // Set spawn point from the NEW room
                if (_pendingTransitionDirection == 1)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromLeft");
                else if (_pendingTransitionDirection == -1)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromRight");
                else if (_pendingTransitionDirection == 2)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromDown");
                else if (_pendingTransitionDirection == -2)
                    player.Position = levels.currentRoom.GetSpawnPoint("fromUp");

                player.Velocity = Vector2.Zero;
                _camera.SnapTo(player.Position);
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
            _gameOverAlpha = 0f;
            Reset();
        }
        _previousMouse = ms;
    }

    private void UpdateWin(GameTime gameTime)
    {
        if (_winAlpha < 1f)
            _winAlpha = MathHelper.Clamp(
                _winAlpha + FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f, 1f);

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

    /// <returns>true if the charm inventory consumed this frame (caller should return early)</returns>
    private bool UpdateCharmInventory()
    {
        KeyboardState ks = Keyboard.GetState();
        if (ks.IsKeyDown(Keys.I) && _prevKeyboard.IsKeyUp(Keys.I) && !_isShopOpen)
            _charmInventoryOpen = !_charmInventoryOpen;
        _prevKeyboard = ks;

        if (!_charmInventoryOpen)
        {
            _charmDenyTimer = 0f;
            return false;
        }

        if (_charmDenyTimer > 0f)
            _charmDenyTimer -= 1f / 60f;

        MouseState ms = Mouse.GetState();
        if (ms.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            HandleCharmClick(ms.Position);
        _previousMouse = ms;
        return true;
    }

    private bool IsShopRoom()
    {
        string name = levels.currentRoom.roomName;
        return name == "shop" || name == "shop2";
    }

    private bool UpdateShop()
    {
        KeyboardState ks = Keyboard.GetState();
        bool inShopRoom = IsShopRoom();

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

        // Apply ALL movement together, then resolve collisions
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        player.ApplyPhysics(dt);

        // Only apply horizontal velocity if not blocked by a wall in that direction
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
                if (player.GeoCount >= 150)
                    AchievementManager.Unlock(AchievementManager.Monopoly);
            }
        }

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Update(gameTime);

        itemManager.Update(gameTime);
        _camera.Follow(player.Position);
    }

    // ------------------------------------------------------------------
    //  Draw
    // ------------------------------------------------------------------

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Pass 1 — world space
        spriteBatch.Begin(transformMatrix: _camera.GetTransform());
        levels.Draw(spriteBatch);

        foreach (var geo in levels.currentGeos)
            geo.Draw(spriteBatch);
        player.Draw(spriteBatch, gameTime);
        levels.DrawEnemies(spriteBatch);
        spriteBatch.End();

        // Pass 2 — soul meter & HP masks (non-premultiplied textures)
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        DrawSoulMeter(spriteBatch);
        DrawHPBar(spriteBatch);
        spriteBatch.End();

        // Pass 3 — screen-space HUD & overlays
        spriteBatch.Begin();
        if (_isPaused) DrawPauseScreen(spriteBatch);
        abilityBar.Draw(spriteBatch, _graphics.Viewport.Width, _graphics.Viewport.Height);
        HUD.DrawHUD(player, spriteBatch, _graphics.Viewport.Width, font, levels.geoTexture);
        if (_isGameOver) DrawGameOver(spriteBatch);
        if (_isWin) DrawWinScreen(spriteBatch);
        if (_charmInventoryOpen) DrawCharmInventory(spriteBatch);
        if (_isShopOpen) DrawShopHUD(spriteBatch);
        if (_isTransitioning)
            spriteBatch.Draw(pixelTexture,
                new Rectangle(0, 0, _graphics.Viewport.Width, _graphics.Viewport.Height),
                Color.Black * _transitionAlpha);
        if (itemManager.IsEquipped(WaywardCompassIndex))
            Minimap.Draw(spriteBatch, pixelTexture, _graphics,
                 levels.currentRoom.Bounds, levels.currentRoom.Tiles,
                 player.Position,
                 levels.TotalRooms,
                 levels.CurrentRoomIndex);

        if (IsShopRoom() && !_isShopOpen && !_charmInventoryOpen
            && !_isGameOver && !_isPaused)
        {
            string shopHint = "Press B to open Shop (only available in shop rooms)";
            Vector2 shopHintSize = font.MeasureString(shopHint);
            spriteBatch.DrawString(font, shopHint,
                new Vector2((_graphics.Viewport.Width - shopHintSize.X) / 2f, 20), Color.Gold);
        }

        if (levels.currentRoom.roomName == "level1" && !_charmInventoryOpen
            && !_isGameOver && !_isPaused && !_isShopOpen)
        {
            string invHint = "Press I to open Inventory (available in all rooms)";
            Vector2 invHintSize = font.MeasureString(invHint);
            spriteBatch.DrawString(font, invHint,
                new Vector2((_graphics.Viewport.Width - invHintSize.X) / 2f, 20), Color.Gold);
        }

        spriteBatch.End();
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
        _charmInventoryOpen = false;
        _isShopOpen = false;

        levels.ResetToFirstLevel();
        levels.ResetAllEnemies();
        levels.ClearGeos();

        player = CreatePlayer();
        WireEnemyCallbacks();

        itemManager = new ItemManager(0.4f);
        LoadItems();
        itemManager.VisibleCount = GetVisibleCharmCount();
        itemManager.EquipItem(0, player);
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
    }
}