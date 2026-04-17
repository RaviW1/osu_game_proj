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
                // Enemy collision
                if (currentEnemy is IEnemy enemyCollision && !enemyCollision.IsDead)
                {
                    var enemyVelocity = new Vector2(enemyCollision.GetVelocityX(), enemyCollision.GetVelocityY());
                    var enemyResults = CollisionSystem.Query(enemyCollision.GetBounds(), _grid, enemyVelocity);
                    enemyCollision.ResolveCollisions(enemyResults);
                }

                // Enemy update
                currentEnemy.Update(gameTime);

                // Enemy body vs player
                Rectangle playerBounds = player.GetBounds();
                if (currentEnemy is IEnemy enemy && !enemy.IsDead)
                {
                    if (enemy.GetBounds().Intersects(playerBounds))
                    {
                        if (!player.IsInvincible)
                        {
                            player.PlayerHealth--;
                            player.TakeDamage();
                            OnPlayerHit?.Invoke();
                        }
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
                if (currentEnemy is IEnemy targetEnemy && !targetEnemy.IsDead)
                {
                    for (int i = player.Projectiles.Count - 1; i >= 0; i--)
                    {
                        if (player.Projectiles[i].GetBounds().Intersects(targetEnemy.GetBounds()))
                        {
                            bool wasAlive = !targetEnemy.IsDead;
                            targetEnemy.TakeDamage();
                            OnEnemyHit?.Invoke();
                            player.Soul = System.Math.Min(player.Soul + 10, player.SoulLimit);
                            if (wasAlive && targetEnemy.IsDead)
                                PendingDeathPositions.Add(new Vector2(targetEnemy.GetBounds().Center.X, targetEnemy.GetBounds().Center.Y));
                            player.Projectiles.RemoveAt(i);
                        }
                    }
                }

                // Melee vs enemy
                if (player.IsAttacking && currentEnemy is IEnemy meleeTarget && !meleeTarget.IsDead)
                {
                    if (player.GetMeleeHitbox().Intersects(meleeTarget.GetBounds()))
                    {
                        bool wasAlive = !meleeTarget.IsDead;
                        meleeTarget.TakeDamage();
                        OnEnemyHit?.Invoke();
                        player.Soul = System.Math.Min(player.Soul + 10, player.SoulLimit);
                        if (wasAlive && meleeTarget.IsDead)
                            PendingDeathPositions.Add(new Vector2(meleeTarget.GetBounds().Center.X, meleeTarget.GetBounds().Center.Y));
                    }
                }
            }
        }
    }
}