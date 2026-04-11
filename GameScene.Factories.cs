using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using osu_game_proj;
using System;
using System.Collections.Generic;

public partial class GameScene
{
    private static readonly Random _rng = new Random();

    private void ProcessInput(GameTime gameTime)
    {
        foreach (ICommand cmd in keyboard.GetCommands(gameTime))
            cmd.Execute(player, gameTime);
        foreach (ICommand cmd in mouse.GetCommands(gameTime))
            cmd.Execute(player, gameTime);
    }

    private void SpawnDeathGeos()
    {
        var deathPositions = levels.currentEnemyGen.PendingDeathPositions;
        if (deathPositions.Count == 0) return;

        foreach (Vector2 pos in deathPositions)
        {
            int count = _rng.Next(1, 4);
            for (int i = 0; i < count; i++)
            {
                float xSpread = _rng.Next(-20, 21);
                float yVel = _rng.Next(-250, -100);
                Vector2 spawnPos = new Vector2(pos.X + xSpread, pos.Y);
                Vector2 vel = new Vector2(xSpread * 2, yVel);
                levels.currentGeos.Add(new Geo(levels.geoTexture, spawnPos, vel));
            }
        }
        deathPositions.Clear();
    }

    private Player CreatePlayer()
    {
        var textures = new Dictionary<string, Texture2D>
        {
            { "Walking", _content.Load<Texture2D>("hollow_knight_walking") },
            { "Jumping", _content.Load<Texture2D>("knight_jumping") },
            { "Attacking", _content.Load<Texture2D>("knight_attack") },
            { "Attack", _content.Load<Texture2D>("hollow_knight_attack") },
            { "SpriteSheet", _content.Load<Texture2D>("The Knight main sprites - atlas0 #00000357") },
            { "lookup", _content.Load<Texture2D>("lookup") }
        };
        return new Player(textures, fireballTexture, new Vector2(350, 370));
    }

    private List<ISprite> CreateBlocks()
    {
        Texture2D spikeTex = _content.Load<Texture2D>("spike_back");
        Texture2D fungalSpikeTex = _content.Load<Texture2D>("fungd_spikes_01");

        return new List<ISprite>
        {
            new MapBlock(spikeTex, new Vector2(50, 50)),
            new MapBlock(fungalSpikeTex, new Vector2(50, 50))
        };
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
            new TextureItem(0, heartTex, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2));
        itemManager.AddItem(
            new TextureItem(1, dashTex, p => p.CanDash = true, p => p.CanDash = false));
    }

    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(_graphics, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
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
}
