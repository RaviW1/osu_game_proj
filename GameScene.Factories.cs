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
    private void CheckPuzzleSolved()
    {
        if (_puzzleSolved) return;
        if (_pushBoxes.Count < 3) return;
        bool allRight = _pushBoxes.TrueForAll(b => b.Position.X >= 840);
        if (allRight)
        {
            _puzzleSolved = true;
            CycleStage(-2);
        }
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
            { "lookup", _content.Load<Texture2D>("lookup") },
            { "side_slash", _content.Load<Texture2D>("slash_effect_sideways") }
        };
        var p = new Player(textures, fireballTexture, new Vector2(350, 370));
        p.OnDamaged = () => TriggerHitEffects(playerWasHit: true);
        return p;  // return p, not a new Player
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
        Texture2D heartTex = _content.Load<Texture2D>("Charms/Unbreakable Heart - _0002_charm_glass_heal_full");
        Texture2D dashTex = _content.Load<Texture2D>("Charms/Dashmaster_0011_charm_generic_03");
        Texture2D compassTex = _content.Load<Texture2D>("Charms/WaywardCompass");

        itemManager.AddItem(
            new TextureItem(0, heartTex, p => p.MaxPlayerHealth += 2, p => p.MaxPlayerHealth -= 2));
        itemManager.AddItem(
            new TextureItem(1, dashTex, p => p.CanDash = true, p => p.CanDash = false));
        itemManager.AddItem(
            new TextureItem(2, compassTex));

        // Unbreakable Heart is equipped by default at game start
        itemManager.EquipItem(0, player);
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
    private void SpawnPushBoxes()
    {
        // On left platform (centered)
        _pushBoxes.Add(new PushBox(pixelTexture, new Vector2(296, 340)));
        // On right platform (centered)
        _pushBoxes.Add(new PushBox(pixelTexture, new Vector2(566, 340)));
        // On ground
        _pushBoxes.Add(new PushBox(pixelTexture, new Vector2(420, 455)));
    }
    private void UpdatePushBoxes(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var box in _pushBoxes)
        {
            box.Update(gameTime);

            foreach (var tile in levels.currentRoom.Tiles)
            {
                if (!tile.isCollideable) continue;
                Rectangle overlap = Rectangle.Intersect(box.GetBounds(), tile.bounds);
                if (overlap.IsEmpty) continue;

                if (overlap.Width < overlap.Height)
                {

                    box.Position.X += box.GetBounds().Center.X < tile.bounds.Center.X
                        ? -overlap.Width : overlap.Width;
                    box.Velocity.X = 0;
                }
                else
                {

                    box.Position.Y += box.GetBounds().Center.Y < tile.bounds.Center.Y
                        ? -overlap.Height : overlap.Height;
                    box.Velocity.Y = 0;
                }
            }


            //  box against other boxes
            for (int i = 0; i < _pushBoxes.Count; i++)
            {
                for (int j = i + 1; j < _pushBoxes.Count; j++)
                {
                    Rectangle overlap = Rectangle.Intersect(_pushBoxes[i].GetBounds(), _pushBoxes[j].GetBounds());
                    if (overlap.IsEmpty) continue;

                    if (overlap.Width <= overlap.Height)
                    {
                        float push = overlap.Width / 2f;
                        if (_pushBoxes[i].GetBounds().Center.X < _pushBoxes[j].GetBounds().Center.X)
                        { _pushBoxes[i].Position.X -= push; _pushBoxes[j].Position.X += push; }
                        else
                        { _pushBoxes[i].Position.X += push; _pushBoxes[j].Position.X -= push; }
                    }
                    else
                    {
                        if (_pushBoxes[i].GetBounds().Center.Y < _pushBoxes[j].GetBounds().Center.Y)
                        { _pushBoxes[i].Position.Y -= overlap.Height; _pushBoxes[j].Velocity.Y = 0; }
                        else
                        { _pushBoxes[j].Position.Y -= overlap.Height; _pushBoxes[i].Velocity.Y = 0; }
                    }
                    _pushBoxes[i].Velocity.X = 0;
                    _pushBoxes[j].Velocity.X = 0;
                }
            }

            // Player pushes box
            Rectangle playerBounds = player.GetBounds();
            Rectangle boxBounds = box.GetBounds();
            if (playerBounds.Intersects(boxBounds))
            {
                Rectangle overlap = Rectangle.Intersect(playerBounds, boxBounds);
                if (overlap.Width < overlap.Height)
                {
                    // Player is pushing horizontally
                    if (playerBounds.Center.X < boxBounds.Center.X)
                    {
                        box.Position.X += overlap.Width;
                        box.Velocity.X = 0;
                    }
                    else
                    {
                        box.Position.X -= overlap.Width;
                        box.Velocity.X = 0;
                    }
                }
                else
                {
                    // Player lands on top of box
                    if (playerBounds.Center.Y < boxBounds.Center.Y)
                        player.Position.Y -= overlap.Height;
                }
            }
        }
        // Second pass — carry stacked boxes after all movement is resolved
        float[] deltas = new float[_pushBoxes.Count];
        for (int i = 0; i < _pushBoxes.Count; i++)
            deltas[i] = _pushBoxes[i].Position.X - _pushBoxes[i].LastPosition.X;

        for (int pass = 0; pass < _pushBoxes.Count; pass++)
        {
            for (int i = 0; i < _pushBoxes.Count; i++)
            {
                for (int j = 0; j < _pushBoxes.Count; j++)
                {
                    if (i == j) continue;
                    Rectangle top = _pushBoxes[j].GetBounds();
                    Rectangle bottom = _pushBoxes[i].GetBounds();

                    bool sittingOnTop = Math.Abs(top.Bottom - bottom.Top) < 6
                                        && top.Right > bottom.Left + 2
                                        && top.Left < bottom.Right - 2;

                    if (sittingOnTop)
                    {
                        _pushBoxes[j].Position.Y = bottom.Top - PushBox.Size;
                        _pushBoxes[j].Velocity.Y = 0;

                        if (Math.Abs(deltas[i]) > 0.01f)
                        {
                            _pushBoxes[j].Position.X += deltas[i];
                            deltas[j] = deltas[i];
                            deltas[i] = 0;
                        }
                    }
                }
            }
        }
        CheckPuzzleSolved();

    }
}
