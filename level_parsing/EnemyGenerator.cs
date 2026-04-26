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
        public List<Vector2> PendingDeathPositions { get; } = new List<Vector2>();

        public Action OnPlayerHit;
        public Action OnEnemyHit;

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
                {
                    enemy = new Boofly(enemyTextures["boofly"], enemyInfo.destPos);
                }
                else if (enemyInfo.enemyType == "aspid")
                {
                    enemy = new Aspid(enemyTextures["aspid"], fireballTexture, enemyInfo.destPos);
                }
                else if (enemyInfo.enemyType == "husk_bully")
                {
                    enemy = new HuskBully(enemyTextures["husk_bully"], enemyInfo.destPos);
                }
                else if (enemyInfo.enemyType == "false_knight")
                {
                    enemy = new Boss(enemyTextures["false_knight"], enemyInfo.destPos);
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
            foreach (IEnemy currentEnemy in this.enemyList)
            {
                // Run collision on ALL enemies - alive AND dead
                // Dead enemies need collision to find the floor and stop falling
                var enemyVelocity = new Vector2(currentEnemy.GetVelocityX(), currentEnemy.GetVelocityY());
                var enemyResults = CollisionSystem.Query(currentEnemy.GetBounds(), _grid, enemyVelocity);
                currentEnemy.ResolveCollisions(enemyResults);

                // Update movement/animation
                currentEnemy.Update(gameTime);

                // Skip all combat logic for dead or phased enemies
                if (currentEnemy.IsDead || currentEnemy.IsPhased) continue;

                // Enemy body vs player
                Rectangle playerBounds = player.GetBounds();
                if (currentEnemy.GetBounds().Intersects(playerBounds))
                {
                    if (!player.IsInvincible)
                    {
                        player.PlayerHealth--;
                        player.TakeDamage();
                        OnPlayerHit?.Invoke();
                    }
                }

                // Aspid projectiles vs player
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

                // Player projectiles vs enemy
                for (int i = player.Projectiles.Count - 1; i >= 0; i--)
                {
                    if (player.Projectiles[i].GetBounds().Intersects(currentEnemy.GetBounds()))
                    {
                        bool wasAlive = !currentEnemy.IsDead;
                        currentEnemy.TakeDamage();
                        OnEnemyHit?.Invoke();
                        player.Soul = Math.Min(player.Soul + 10, player.SoulLimit);
                        if (wasAlive && currentEnemy.IsDead)
                            PendingDeathPositions.Add(new Vector2(currentEnemy.GetBounds().Center.X, currentEnemy.GetBounds().Center.Y));
                        player.Projectiles.RemoveAt(i);
                        break;
                    }
                }

                // Melee vs enemy
                if (player.IsAttacking)
                {
                    if (player.GetMeleeHitbox().Intersects(currentEnemy.GetBounds()))
                    {
                        bool wasAlive = !currentEnemy.IsDead;
                        currentEnemy.TakeDamage();
                        OnEnemyHit?.Invoke();
                        player.Soul = Math.Min(player.Soul + 10, player.SoulLimit);
                        if (wasAlive && currentEnemy.IsDead)
                            PendingDeathPositions.Add(new Vector2(currentEnemy.GetBounds().Center.X, currentEnemy.GetBounds().Center.Y));
                    }
                }
            }
        }
    }
}
