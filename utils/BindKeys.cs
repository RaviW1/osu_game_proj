using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class BindKeys
    {
        private KeyboardController keyboard;

        public BindKeys(KeyboardController keyboard)
        {
            this.keyboard = keyboard;
        }

        public void bindKeys(GameScene scene, Game1 game)
        {
            // Movement: arrow keys only (WASD removed so A/W/S/D can be used as action keys)
            var moveAxisCmd = new MovementAxisCommand(
                leftKeys: new[] { Keys.Left },
                rightKeys: new[] { Keys.Right }
            );
            var vertAxisCmd = new VerticalAxisCommand(
                upKeys: new[] { Keys.Up },
                downKeys: new[] { Keys.Down }
            );
            var jumpPressedCmd = new JumpPressedCommand();
            var jumpHeldCmd = new JumpHeldCommand();

            keyboard.BindHeld(Keys.Left, moveAxisCmd);
            keyboard.BindHeld(Keys.Right, moveAxisCmd);
            keyboard.BindRelease(Keys.Left, moveAxisCmd);
            keyboard.BindRelease(Keys.Right, moveAxisCmd);

            keyboard.BindHeld(Keys.Up, vertAxisCmd);
            keyboard.BindHeld(Keys.Down, vertAxisCmd);
            keyboard.BindRelease(Keys.Up, vertAxisCmd);
            keyboard.BindRelease(Keys.Down, vertAxisCmd);

            // Hollow Knight-style action layout
            keyboard.BindPress(Keys.Z, jumpPressedCmd);
            keyboard.BindHeld(Keys.Z, jumpHeldCmd);
            keyboard.BindPress(Keys.X, new AttackCommand());
            keyboard.BindHeld(Keys.A, new HealCommand());
            keyboard.BindRelease(Keys.A, new InterruptHealCommand());
            keyboard.BindPress(Keys.F, new ShootFireballCommand());
            keyboard.BindPress(Keys.C, new DashCommand());
            keyboard.BindPress(Keys.Escape, new PauseCommand(scene));
            // Inventory (I) is handled in GameScene.UpdateCharmInventory

            // Utility / debug
            keyboard.BindPress(Keys.M, new MuteCommand());
            keyboard.BindPress(Keys.R, new ResetCommand(scene));
            keyboard.BindPress(Keys.Delete, new QuitCommand(game));
        }
    }
}
