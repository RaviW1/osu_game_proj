 using Microsoft.Xna.Framework;
using System.Collections.Generic;
using osu_game_proj;


public static class PhysicsHelper
{
    // CURRENTLY UNUSED
    // IVE KEPT THESE FUNCTIONS JUST IN CASE
    // Returns true if there is a solid tile directly below the player's feet
    public static bool HasGroundBelow(Player player)
    {
        Rectangle feet = new Rectangle(
            player.GetBounds().X,
            player.GetBounds().Bottom,
            player.GetBounds().Width,
            4);

        foreach (Rectangle tile in Game1.GetCurrentLevelColliders())
            if (feet.Intersects(tile)) return true;

        return false;
    }

    // CURRENTLY UNUSED
    // IVE KEPT THESE FUNCTIONS JUST IN CASE
    // Returns true if player is falling and their feet are near the top of a tile
    public static bool IsLandingOnTile(Player player, Rectangle tile)
    {
        Rectangle playerBounds = player.GetBounds();
        bool isFalling = player.Velocity.Y > 0;
        bool feetNearTop = playerBounds.Bottom - tile.Top < 20;

        return isFalling && feetNearTop;
    }

    // CURRENTLY UNUSED
    // IVE KEPT THESE FUNCTIONS JUST IN CASE
    // Snaps player to the top of a tile and zeroes vertical velocity
    public static void LandOnTile(Player player, Rectangle tile)
    {
        Rectangle playerBounds = player.GetBounds();
        player.Position.Y = tile.Top - (playerBounds.Height / 2f);
        player.Velocity.Y = 0f;
        player.IsAirborne = false;
    }
    public static void CheckCollisions(Player player, TileGenerator tileGen)
    {
        Rectangle playerBound = player.GetBounds();

        var tiles = tileGen.TileList;

        foreach (TileBlock tile in tiles)
        {
            if (tile.isCollideable || tile.isHarmful)
            {
                if (playerBound.Intersects(tile.bounds))
                {
                    player.HandleOverlap(tile.bounds);
                }
            }
            if (tile.isHarmful)
            {
                if (playerBound.Intersects(tile.bounds))
                {
                    player.TakeDamage();
                }
            }
        }


        // TODO: figure out how to merge these two loops
        Rectangle feet = new Rectangle(
            player.GetBounds().X,
            player.GetBounds().Bottom,
            player.GetBounds().Width,
            4);

        bool grounded = false;
        foreach (TileBlock tile in tiles)
        {
            if ((tile.isCollideable || tile.isHarmful) && feet.Intersects(tile.bounds))
            {
                grounded = true;
            }
        }
        player.OnGround = grounded;
    }

    public static void CheckEnemyCollisions(Player player, List<ISprite> enemies, int currentEnemyIndex, TileGenerator tileGen)
    {

        var handler = new ProjectilePlayerCollisionHandler();
        Rectangle playerBounds = player.GetBounds();

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
        // Melee hitbox vs enemies
        if (player.IsAttacking)
        {
            Rectangle meleeHitbox = player.GetMeleeHitbox();
            if (currentEnemy is Aspid aspidMelee && !aspidMelee.IsDead)
            {
                if (meleeHitbox.Intersects(aspidMelee.GetBounds()))
                {
                    aspidMelee.TakeDamage();
                }
            }
            else if (currentEnemy is Boofly booflyMelee && !booflyMelee.IsDead)
            {
                if (meleeHitbox.Intersects(booflyMelee.GetBounds()))
                {
                    booflyMelee.TakeDamage();
                }
            }
            else if (currentEnemy is HuskBully huskBullyMelee && !huskBullyMelee.IsDead)
            {
                if (meleeHitbox.Intersects(huskBullyMelee.GetBounds()))
                {
                    huskBullyMelee.TakeDamage();
                }
            }
        }

        // Enemy/block collisions
        foreach (TileBlock tile in tileGen.TileList){
            if (!tile.isCollideable) continue;
            if (currentEnemy is Aspid aspidB && !aspidB.IsDead){
                Rectangle b = aspidB.GetBounds();
                if (b.Intersects(tile.bounds)){
                    Rectangle overlap = Rectangle.Intersect(b, tile.bounds);
                    bool isSideCollision = overlap.Width < overlap.Height;

                    if (isSideCollision){
                        bool hitFromLeft = b.Center.X < tile.bounds.Center.X;
                        if ((hitFromLeft && aspidB.GetVelocityX() > 0) || (!hitFromLeft && aspidB.GetVelocityX() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                aspidB.TakeDamage();
                            }
                            else
                            {
                                aspidB.BounceX();
                            }
                        }
                    }else{
                        bool hitFromTop = b.Center.Y < tile.bounds.Center.Y;
                        if ((hitFromTop && aspidB.GetVelocityY() > 0) || (!hitFromTop && aspidB.GetVelocityY() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                aspidB.TakeDamage();
                            }
                            else
                            {
                                aspidB.BounceY();
                            }
                        }
                    }
                }
            }else if (currentEnemy is Boofly booflyB && !booflyB.IsDead){
                Rectangle b = booflyB.GetBounds();
                if (b.Intersects(tile.bounds)){
                    Rectangle overlap = Rectangle.Intersect(b, tile.bounds);
                    bool isSideCollision = overlap.Width < overlap.Height;

                    if (isSideCollision){
                        bool hitFromLeft = b.Center.X < tile.bounds.Center.X;
                        if ((hitFromLeft && booflyB.GetVelocityX() > 0) || (!hitFromLeft && booflyB.GetVelocityX() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                booflyB.TakeDamage();
                            }
                            else
                            {
                                booflyB.BounceX();
                            }
                        }
                    }
                    else{
                        bool hitFromTop = b.Center.Y < tile.bounds.Center.Y;
                        if ((hitFromTop && booflyB.GetVelocityY() > 0) || (!hitFromTop && booflyB.GetVelocityY() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                booflyB.TakeDamage();
                            }
                            else
                            {
                                booflyB.BounceY();
                            }
                        }
                    }
                }
            }
            else if (currentEnemy is HuskBully huskBullyB && !huskBullyB.IsDead)
            {
                Rectangle b = huskBullyB.GetBounds();
                if (b.Intersects(tile.bounds))
                {
                    Rectangle overlap = Rectangle.Intersect(b, tile.bounds);
                    bool isSideCollision = overlap.Width < overlap.Height;

                    if (isSideCollision)
                    {
                        bool hitFromLeft = b.Center.X < tile.bounds.Center.X;
                        if ((hitFromLeft && huskBullyB.GetVelocityX() > 0) || (!hitFromLeft && huskBullyB.GetVelocityX() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                huskBullyB.TakeDamage();
                            }
                            else
                            {
                                huskBullyB.BounceX();
                            }
                        }
                    }
                    else
                    {
                        bool hitFromTop = b.Center.Y < tile.bounds.Center.Y;
                        if ((hitFromTop && huskBullyB.GetVelocityY() > 0) || (!hitFromTop && huskBullyB.GetVelocityY() < 0))
                        {
                            if (tile.isHarmful)
                            {
                                huskBullyB.TakeDamage();
                            }
                            else
                            {
                                huskBullyB.BounceY();
                            }
                        }
                    }
                }
            }
        }
    }
    public static void CheckPlayerGeosCollisions(Player player, List<Geo> geos, GameTime gameTime)
    {
        Rectangle playerBounds = player.GetBounds();
        for (int i = geos.Count - 1; i >= 0; i--)
        {
            if (!geos[i].IsCollected && geos[i].GetBounds().Intersects(playerBounds))
            {
                geos[i].Collect();
                player.GeoCount++;
            }
            geos[i].Update(gameTime);
        }
    }
}
