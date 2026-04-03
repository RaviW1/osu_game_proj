using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using osu_game_proj;
using System.Collections.Generic;

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

    // Rooms
    private RoomBase _currentRoom;
    private RoomBase _roomA;
    private RoomBase _roomB;

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

        // Rooms
        _roomA = new RoomA();
        _roomB = new RoomB();
        _roomA.Load(_content);
        _roomB.Load(_content);
        _currentRoom = _roomA;

        // Geos
        geoTexture = _content.Load<Texture2D>("Geo - HUD_coin_shop");
        geosLevel1 = new List<Geo>();
        geosLevel2 = new List<Geo>();
        Geo.PlaceGeosOnPlatforms(_roomA.Tiles, geosLevel1, geoTexture);
        Geo.PlaceGeosOnPlatforms(_roomB.Tiles, geosLevel2, geoTexture);
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
        _camera.RoomBounds = _currentRoom.Bounds;
        _camera.SnapTo(player.Position);
    }

    public void Update(GameTime gameTime)
    {
        Rectangle playerBounds = player.GetBounds();
        ISprite currentEnemy = enemies[currentEnemyIndex];

        // 1. Collision first — sets OnGround
        _currentRoom.Update(gameTime, player);

        // 2. Enemy collision
        if (currentEnemy is IEnemy enemyCollision && !enemyCollision.IsDead)
        {
            var enemyVelocity = new Vector2(enemyCollision.GetVelocityX(), enemyCollision.GetVelocityY());
            var enemyResults = CollisionSystem.Query(enemyCollision.GetBounds(), _currentRoom.Tiles, enemyVelocity);
            enemyCollision.ResolveCollisions(enemyResults);
        }

        // 3. Input next — sets commandReceivedThisFrame before player.Update reads it
        List<ICommand> currentCommands = keyboard.GetCommands(gameTime);
        foreach (ICommand command in currentCommands)
            command.Execute(player, gameTime);

        List<ICommand> currentMouseCommands = mouse.GetCommands(gameTime);
        foreach (ICommand command in currentMouseCommands)
            command.Execute(player, gameTime);

        // 4. Player update — states now see correct OnGround AND correct input
        player.Update(gameTime);

        // 5. Enemy update
        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Update(gameTime);

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
        for (int i = geos.Count - 1; i >= 0; i--)
        {
            if (!geos[i].IsCollected && geos[i].GetBounds().Intersects(playerBounds))
            {
                geos[i].Collect();
                player.GeoCount++;
            }
            geos[i].Update(gameTime);
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

        _currentRoom.Draw(spriteBatch);

        if (enemies.Count > 0)
            enemies[currentEnemyIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);

        if (blocks.Count > 0)
            blocks[currentBlockIndex].Draw(spriteBatch, System.Numerics.Vector2.Zero);

        foreach (var geo in geos)
            geo.Draw(spriteBatch);

        player.Draw(spriteBatch, gameTime);

        spriteBatch.End();

        // Pass 2 — screen space HUD
        spriteBatch.Begin();

        abilityBar.Draw(spriteBatch, _graphics.Viewport.Width, _graphics.Viewport.Height);
        itemManager.Draw(spriteBatch);
        HUD.DrawHUD(player, spriteBatch, _graphics.Viewport.Width, font);

        spriteBatch.End();
    }

    public void Unload() { }

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
            _currentRoom = _roomA;
            geos = geosLevel1;
        }
        else if (direction == 1)
        {
            _currentRoom = _roomB;
            geos = geosLevel2;
        }

        _camera.RoomBounds = _currentRoom.Bounds;
        _camera.SnapTo(player.Position);
    }

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
        Geo.PlaceGeosOnPlatforms(_roomA.Tiles, geosLevel1, geoTexture);
        Geo.PlaceGeosOnPlatforms(_roomB.Tiles, geosLevel2, geoTexture);
        geos = (_currentRoom == _roomA) ? geosLevel1 : geosLevel2;

        currentEnemyIndex = 0;
        currentBlockIndex = 0;

        _camera.SnapTo(player.Position);
    }

    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(_graphics, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}