using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Collections.Generic;
using System.Xml.Linq;

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
    private List<Geo> geos;
    private List<Geo> geosLevel1;
    private List<Geo> geosLevel2;
    private Texture2D geoTexture;
    private LoadLevelFile level1FileLoader;
    private LoadLevelFile level2FileLoader;
    private TileGenerator tileGenObj1;
    private TileGenerator tileGenObj2;
    private TileGenerator drawTilesGen;

    // Camera
    private Camera2D _camera;

    public GameScene(GraphicsDevice graphics, ContentManager content, Game1 game)
    {
        _graphics = graphics;
        _content = content;
        _game = game;
    }

    public void Load()
    {
        // Input
        keyboard = new KeyboardController();
        BindKeys keyBindObj = new BindKeys(keyboard);
        keyBindObj.bindKeys(this, _game);  // ONE call, passing both

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

        // Enemies
        enemies = new List<ISprite>();
        Texture2D enemyTexture = _content.Load<Texture2D>("boofly");
        enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
        Texture2D aspidTexture = _content.Load<Texture2D>("Aspid");
        Texture2D fireballTexture = _content.Load<Texture2D>("fireball");
        enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));
        Texture2D huskBullyTexture = _content.Load<Texture2D>("husk_bully");
        enemies.Add(new HuskBully(huskBullyTexture, new System.Numerics.Vector2(100, 360)));

        // UI
        pixelTexture = CreatePixelTexture();
        var abilityIcons = new Dictionary<string, Texture2D>();
        abilityIcons.Add("Attack", _content.Load<Texture2D>("hollow_knight_attack"));
        abilityIcons.Add("Fireball", _content.Load<Texture2D>("fireball"));
        abilityIcons.Add("Heal", _content.Load<Texture2D>("hollow_knight_walking"));

        var iconSourceRects = new Dictionary<string, Rectangle?>();
        iconSourceRects.Add("Attack", new Rectangle(896, 0, 128, 128));
        iconSourceRects.Add("Fireball", new Rectangle(0, 0, fireballTexture.Width / 2, fireballTexture.Height / 2));
        Texture2D playerTexture = _content.Load<Texture2D>("hollow_knight_walking");
        iconSourceRects.Add("Heal", new Rectangle(0, 0, playerTexture.Width / 8, playerTexture.Height));
        abilityBar = new AbilityBar(pixelTexture, abilityIcons, iconSourceRects, Vector2.Zero);

        // Blocks
        blocks = new List<ISprite>();
        Texture2D spikeTexture = _content.Load<Texture2D>("spike_back");
        blocks.Add(new MapBlock(spikeTexture, new System.Numerics.Vector2(50, 50)));
        Texture2D fungalSpikeTexture = _content.Load<Texture2D>("fungd_spikes_01");
        blocks.Add(new MapBlock(fungalSpikeTexture, new System.Numerics.Vector2(50, 50)));

        // Tiles
        List<TileInformation> generateTileInfo = new List<TileInformation>();
        level1FileLoader = new LoadLevelFile();
        level1FileLoader.LoadFile("level_files/test_level.xml", generateTileInfo);
        tileGenObj1 = new TileGenerator(new List<TileInformation>(generateTileInfo));
        tileGenObj1.LoadTileTextures(_content);
        generateTileInfo.Clear();

        level2FileLoader = new LoadLevelFile();
        level2FileLoader.LoadFile("level_files/test_level2.xml", generateTileInfo);
        tileGenObj2 = new TileGenerator(new List<TileInformation>(generateTileInfo));
        tileGenObj2.LoadTileTextures(_content);
        drawTilesGen = tileGenObj1;

        // Geos
        geoTexture = _content.Load<Texture2D>("Geo - HUD_coin_shop");
        geosLevel1 = new List<Geo>();
        geosLevel2 = new List<Geo>();
        Geo.PlaceGeosOnPlatforms(tileGenObj1, geosLevel1, geoTexture);
        Geo.PlaceGeosOnPlatforms(tileGenObj2, geosLevel2, geoTexture);
        geos = geosLevel1;

        // Player
        var playerTextures = new Dictionary<string, Texture2D>();
        playerTextures.Add("Walking", _content.Load<Texture2D>("hollow_knight_walking"));
        playerTextures.Add("Jumping", _content.Load<Texture2D>("knight_jumping"));
        playerTextures.Add("Attacking", _content.Load<Texture2D>("knight_attack"));
        playerTextures.Add("Attack", _content.Load<Texture2D>("hollow_knight_attack"));
        player = new Player(playerTextures, fireballTexture, new Vector2(350, 370));

        // Items
        Texture2D unbreakableHeart = _content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
        Texture2D dashmaster = _content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
        itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2), new Vector2(10, 10));
        itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

        font = _content.Load<SpriteFont>("DefaultFont");

        // Camera — always last
        _camera = new Camera2D(_graphics);
        _camera.RoomBounds = new Rectangle(0, 0, 3200, 720);
        _camera.SnapTo(player.Position);
    }

    public void Update(GameTime gameTime)
    {
        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Update(gameTime);

        Rectangle playerBounds = player.GetBounds();
        var handler = new ProjectilePlayerCollisionHandler();

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

        if (player.IsAttacking)
        {
            Rectangle meleeHitbox = player.GetMeleeHitbox();
            if (currentEnemy is Aspid aspidMelee && !aspidMelee.IsDead)
            {
                if (meleeHitbox.Intersects(aspidMelee.GetBounds()))
                    aspidMelee.TakeDamage();
            }
            else if (currentEnemy is Boofly booflyMelee && !booflyMelee.IsDead)
            {
                if (meleeHitbox.Intersects(booflyMelee.GetBounds()))
                    booflyMelee.TakeDamage();
            }
            else if (currentEnemy is HuskBully huskBullyMelee && !huskBullyMelee.IsDead)
            {
                if (meleeHitbox.Intersects(huskBullyMelee.GetBounds()))
                    huskBullyMelee.TakeDamage();
            }
        }

        PhysicsHelper.CheckPlayerGeosCollisions(player, geos, gameTime);

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Update(gameTime);

        player.Update(gameTime);

        List<ICommand> currentCommands = keyboard.GetCommands(gameTime);
        foreach (ICommand command in currentCommands)
            command.Execute(player, gameTime);

        List<ICommand> currentMouseCommands = mouse.GetCommands(gameTime);
        foreach (ICommand command in currentMouseCommands)
            command.Execute(player, gameTime);

        itemManager.Update(gameTime);

        if (drawTilesGen != null)
        {
            PhysicsHelper.CheckCollisions(player, drawTilesGen);
            PhysicsHelper.CheckEnemyCollisions(player, enemies, currentEnemyIndex, drawTilesGen);
        }

        // Camera always last
        _camera.Follow(player.Position);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Pass 1 — world space, moves with camera
        spriteBatch.Begin(transformMatrix: _camera.GetTransform());

        drawTilesGen.Draw(spriteBatch);

        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);

        foreach (var geo in geos)
            geo.Draw(spriteBatch);

        player.Draw(spriteBatch, gameTime);

        spriteBatch.End();

        // Pass 2 — screen space, HUD never moves
        spriteBatch.Begin();

        abilityBar.Draw(spriteBatch, _graphics.Viewport.Width, _graphics.Viewport.Height);
        itemManager.Draw(spriteBatch);
        HUD.DrawHUD(player, spriteBatch, _graphics.Viewport.Width, font);

        spriteBatch.End();
    }

    public void Unload() { }

    // -------------------------------------------------------------------------
    // Stage / enemy / block cycling — replaces old Game1 static methods
    // -------------------------------------------------------------------------
    public void CycleEnemy(int direction)
    {
        if (enemies.Count == 0) return;
        currentEnemyIndex = (currentEnemyIndex + direction + enemies.Count) % enemies.Count;
    }

    public void CycleBlock(int direction)
    {
        if (blocks.Count == 0) return;
        currentBlockIndex = (currentBlockIndex + direction + blocks.Count) % blocks.Count;
    }

    public void CycleStage(int direction)
    {
        if (direction == -1)
        {
            drawTilesGen = tileGenObj1;
            geos = geosLevel1;
        }
        else if (direction == 1)
        {
            drawTilesGen = tileGenObj2;
            geos = geosLevel2;
        }
        _camera.RoomBounds = new Rectangle(0, 0, 3200, 720);
        _camera.SnapTo(player.Position);
    }

    // -------------------------------------------------------------------------
    // Reset
    // -------------------------------------------------------------------------
    public void Reset()
    {
        var playerTextures = new Dictionary<string, Texture2D>();
        playerTextures.Add("Walking", _content.Load<Texture2D>("hollow_knight_walking"));
        playerTextures.Add("Jumping", _content.Load<Texture2D>("knight_jumping"));
        playerTextures.Add("Attacking", _content.Load<Texture2D>("knight_attack"));
        playerTextures.Add("Attack", _content.Load<Texture2D>("hollow_knight_attack"));

        Texture2D fireballTexture = _content.Load<Texture2D>("fireball");
        player = new Player(playerTextures, fireballTexture, new Vector2(350, 370));

        enemies.Clear();
        Texture2D enemyTexture = _content.Load<Texture2D>("boofly");
        enemies.Add(new Boofly(enemyTexture, new System.Numerics.Vector2(500, 50)));
        Texture2D aspidTexture = _content.Load<Texture2D>("Aspid");
        enemies.Add(new Aspid(aspidTexture, fireballTexture, new System.Numerics.Vector2(500, 50)));
        Texture2D huskBullyTexture = _content.Load<Texture2D>("husk_bully");
        enemies.Add(new HuskBully(huskBullyTexture, new System.Numerics.Vector2(100, 360)));

        itemManager = new ItemManager(0.4f);
        Texture2D unbreakableHeart = _content.Load<Texture2D>("Unbreakable Heart - _0002_charm_glass_heal_full");
        Texture2D dashmaster = _content.Load<Texture2D>("Dashmaster_0011_charm_generic_03");
        itemManager.AddItem(new TextureItem(0, unbreakableHeart, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2), new Vector2(10, 10));
        itemManager.AddItem(new TextureItem(1, dashmaster, p => p.CanDash = true, p => p.CanDash = false), new Vector2(100, 10));

        geosLevel1.Clear();
        geosLevel2.Clear();
        Geo.PlaceGeosOnPlatforms(tileGenObj1, geosLevel1, geoTexture);
        Geo.PlaceGeosOnPlatforms(tileGenObj2, geosLevel2, geoTexture);
        geos = (drawTilesGen == tileGenObj1) ? geosLevel1 : geosLevel2;

        currentEnemyIndex = 0;
        currentBlockIndex = 0;

        _camera.SnapTo(player.Position);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------
    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(_graphics, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }

    // DO NOT USE — WILL BE REMOVED SOON
    public List<Rectangle> GetCurrentLevelColliders()
    {
        var rects = new List<Rectangle>();
        foreach (var tile in drawTilesGen.generateTileInfo)
        {
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
