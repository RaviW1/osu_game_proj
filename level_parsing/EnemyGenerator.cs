using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class EnemyGenerator
    {
        private Dictionary<string, Texture2D> enemyTextures;
        public List<EnemyInformation> generateEnemyInfo;
        public Texture2D fireballTexture;
        private List<IEnemy> enemyList;
        public IReadOnlyList<IEnemy> Enemies => enemyList;
        public List<Vector2> PendingDeathPositions { get; } = new List<Vector2>();

        public Action OnPlayerHit;
        public Action OnEnemyHit;
        public Action OnBossDeath;
        public Action OnSecretBossDeath;

        // Tracks enemies already hit this dash so one dash = one hit per enemy
        private HashSet<IEnemy> _dashedThisDash = new HashSet<IEnemy>();

        public EnemyGenerator(List<EnemyInformation> generateEnemyInfo)
        {
            this.generateEnemyInfo = generateEnemyInfo;
        }

        public void LoadEnemyTextures(ContentManager Content)
        {
            enemyTextures = new Dictionary<string, Texture2D>();
            enemyTextures.Add("boofly", Content.Load<Texture2D>("Enemy Sprites\\boofly"));
            enemyTextures.Add("aspid", Content.Load<Texture2D>("Enemy Sprites\\aspid_hunter"));
            enemyTextures.Add("husk_bully", Content.Load<Texture2D>("Enemy Sprites\\husk_bully"));
            enemyTextures.Add("false_knight", Content.Load<Texture2D>("Enemy Sprites\\false_knight"));
            enemyTextures.Add("baldur", Content.Load<Texture2D>("Enemy Sprites\\elder_baldur"));

            fireballTexture = Content.Load<Texture2D>("fireball");
            createEnemyObjects(generateEnemyInfo);
        }

        public void ResetEnemies()
        {
            createEnemyObjects(generateEnemyInfo);
        }

        public void createEnemyObjects(List<EnemyInformation> generateEnemyInfo)
        {
            enemyList = new List<IEnemy>();
            foreach (EnemyInformation enemyInfo in generateEnemyInfo)
            {
                if (!enemyTextures.ContainsKey(enemyInfo.enemyType))
                {
                    System.Console.WriteLine($"MISSING TEXTURE KEY: {enemyInfo.enemyType}");
                    continue;
                }

                IEnemy enemy = new Boofly(enemyTextures["boofly"], enemyInfo.destPos);
                if (enemyInfo.enemyType == "boofly")
                    enemy = new Boofly(enemyTextures["boofly"], enemyInfo.destPos);
                else if (enemyInfo.enemyType == "aspid")
                    enemy = new Aspid(enemyTextures["aspid"], fireballTexture, enemyInfo.destPos);
                else if (enemyInfo.enemyType == "husk_bully")
                    enemy = new HuskBully(enemyTextures["husk_bully"], enemyInfo.destPos);
                else if (enemyInfo.enemyType == "baldur")
                {
                    var baldur = new BaldurBoss(enemyTextures["baldur"], enemyInfo.destPos, fireballTexture);
                    baldur.OnDeath = () => OnSecretBossDeath?.Invoke();
                    enemy = baldur;
                }
                else if (enemyInfo.enemyType == "false_knight")
                {
                    var boss = new Boss(enemyTextures["false_knight"], enemyInfo.destPos);
                    boss.OnDeath = () => OnBossDeath?.Invoke();
                    enemy = boss;
                }
                enemyList.Add(enemy);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IEnemy enemy in this.enemyList)
                enemy.Draw(spriteBatch, Vector2.Zero);
        }

        public void Update(GameTime gameTime, Player player, SpatialGrid _grid)
        {
            // Reset dash hit tracking when dash ends
            if (!player.IsDashing)
                _dashedThisDash.Clear();

            foreach (IEnemy currentEnemy in this.enemyList)
            {
                // Physics collision runs on all enemies alive and dead
                // GetBounds() now returns real bounds for dead enemies so they land on floor
                var enemyBounds = currentEnemy.GetBounds();
                if (enemyBounds != Rectangle.Empty)
                {
                    var enemyVelocity = new Vector2(currentEnemy.GetVelocityX(), currentEnemy.GetVelocityY());
                    var enemyResults = CollisionSystem.Query(enemyBounds, _grid, enemyVelocity);
                    currentEnemy.ResolveCollisions(enemyResults);
                }

                currentEnemy.Update(gameTime);

                // Skip combat for dead or phased enemies
                if (currentEnemy.IsDead || currentEnemy.IsPhased) continue;

                Rectangle playerBounds = player.GetBounds();
                Rectangle liveBounds = currentEnemy.GetBounds();

                // ── Dash damage ───────────────────────────────────────────────
                if (player.IsDashing
                    && !_dashedThisDash.Contains(currentEnemy)
                    && liveBounds.Intersects(playerBounds))
                {
                    _dashedThisDash.Add(currentEnemy);
                    bool wasAlive = !currentEnemy.IsDead;
                    currentEnemy.TakeDamage();
                    OnEnemyHit?.Invoke();
                    player.Soul = Math.Min(player.Soul + Difficulty.SoulPerMeleeHit, player.SoulLimit);
                    if (wasAlive && currentEnemy.IsDead)
                        PendingDeathPositions.Add(new Vector2(liveBounds.Center.X, liveBounds.Center.Y));
                }
                // ── Body collision (not dashing) ──────────────────────────────
                else if (!player.IsDashing && liveBounds.Intersects(playerBounds))
                {
                    if (!player.IsInvincible)
                    {
                        player.PlayerHealth--;
                        player.TakeDamage();
                        OnPlayerHit?.Invoke();
                    }
                }

                // ── Aspid projectiles vs player ───────────────────────────────
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
                                OnPlayerHit?.Invoke();
                            }
                            aspid.Projectiles.RemoveAt(i);
                        }
                    }
                }

                // ── Baldur projectiles vs player ──────────────────────────────
                if (currentEnemy is BaldurBoss baldur)
                {
                    for (int i = baldur.Projectiles.Count - 1; i >= 0; i--)
                    {
                        if (baldur.Projectiles[i].GetBounds().Intersects(playerBounds))
                        {
                            if (!player.IsInvincible)
                            {
                                player.PlayerHealth--;
                                player.TakeDamage();
                                OnPlayerHit?.Invoke();
                            }
                            baldur.Projectiles.RemoveAt(i);
                        }
                    }
                }

                // ── Player projectiles vs enemy ───────────────────────────────
                for (int i = player.Projectiles.Count - 1; i >= 0; i--)
                {
                    if (player.Projectiles[i].GetBounds().Intersects(liveBounds))
                    {
                        bool wasAlive = !currentEnemy.IsDead;
                        currentEnemy.TakeDamage();
                        OnEnemyHit?.Invoke();
                        if (wasAlive && currentEnemy.IsDead)
                            PendingDeathPositions.Add(new Vector2(liveBounds.Center.X, liveBounds.Center.Y));
                        player.Projectiles.RemoveAt(i);
                        break;
                    }
                }

                // ── Melee vs enemy ────────────────────────────────────────────
                if (player.IsAttacking && player.GetMeleeHitbox().Intersects(liveBounds))
                {
                    bool wasAlive = !currentEnemy.IsDead;
                    currentEnemy.TakeDamage();
                    OnEnemyHit?.Invoke();
                    player.Soul = Math.Min(player.Soul + Difficulty.SoulPerMeleeHit, player.SoulLimit);
                    if (wasAlive && currentEnemy.IsDead)
                        PendingDeathPositions.Add(new Vector2(liveBounds.Center.X, liveBounds.Center.Y));
                }
            }

            // Remove corpses once post-death timer expires
            enemyList.RemoveAll(e => e.ShouldBeRemoved);
        }
    }
}