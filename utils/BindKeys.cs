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

            var moveAxisCmd = new MovementAxisCommand(
                leftKeys: new[] { Keys.A, Keys.Left },
                rightKeys: new[] { Keys.D, Keys.Right }
            );
            var vertAxisCmd = new VerticalAxisCommand(
                upKeys: new[] { Keys.W, Keys.Up },
                downKeys: new[] { Keys.S, Keys.Down }
            );
            var jumpPressedCmd = new JumpPressedCommand();
            var jumpHeldCmd = new JumpHeldCommand();

            keyboard.BindHeld(Keys.A, moveAxisCmd);
            keyboard.BindHeld(Keys.Left, moveAxisCmd);
            keyboard.BindHeld(Keys.D, moveAxisCmd);
            keyboard.BindHeld(Keys.Right, moveAxisCmd);
            keyboard.BindRelease(Keys.A, moveAxisCmd);
            keyboard.BindRelease(Keys.Left, moveAxisCmd);
            keyboard.BindRelease(Keys.D, moveAxisCmd);
            keyboard.BindRelease(Keys.Right, moveAxisCmd);

            keyboard.BindHeld(Keys.W, vertAxisCmd);
            keyboard.BindHeld(Keys.Up, vertAxisCmd);
            keyboard.BindHeld(Keys.S, vertAxisCmd);
            keyboard.BindHeld(Keys.Down, vertAxisCmd);
            keyboard.BindRelease(Keys.W, vertAxisCmd);
            keyboard.BindRelease(Keys.Up, vertAxisCmd);
            keyboard.BindRelease(Keys.S, vertAxisCmd);
            keyboard.BindRelease(Keys.Down, vertAxisCmd);

            keyboard.BindPress(Keys.Space, jumpPressedCmd);
            keyboard.BindHeld(Keys.Space, jumpHeldCmd);

            keyboard.BindPress(Keys.LeftShift, new DashCommand());
            keyboard.BindPress(Keys.M, new MuteCommand());

            keyboard.BindPress(Keys.Q, new QuitCommand(game));
            keyboard.BindPress(Keys.R, new ResetCommand(scene));

            keyboard.BindPress(Keys.Z, new AttackCommand());
            keyboard.BindPress(Keys.N, new AttackCommand());
            keyboard.BindPress(Keys.E, new DamageCommand());
            keyboard.BindPress(Keys.D1, new AttackCommand());
            keyboard.BindPress(Keys.D2, new ShootFireballCommand());
            keyboard.BindHeld(Keys.D3, new HealCommand());
            keyboard.BindPress(Keys.D0, new WinCommand(scene));
            keyboard.BindPress(Keys.Escape, new PauseCommand(scene));
        }
    }
}
