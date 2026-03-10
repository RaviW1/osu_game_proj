using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace osu_game_proj
{
    public class BindKeyboardKeys
    {
        private KeyboardController keyboard;
        public BindKeyboardKeys(KeyboardController keyboard)
        {
            this.keyboard = keyboard;
        }
        public void bindKeys(Game1 game1)
        {
            // ------------------------------
            // KEYBINDINGS (client code)
            // Assumes you already created:
            //   KeyboardController keyboard = new KeyboardController();
            // And you have these command classes available:
            //   MovementAxisCommand, VerticalAxisCommand, JumpPressedCommand, JumpReleasedCommand
            // ------------------------------
            keyboard.BindPress(Keys.O, new CycleEnemyCommand(-1));
            keyboard.BindPress(Keys.P, new CycleEnemyCommand(1));


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

            // ---- Horizontal movement axis (X) ----
            // Update while held
            keyboard.BindHeld(Keys.A, moveAxisCmd);
            keyboard.BindHeld(Keys.Left, moveAxisCmd);
            keyboard.BindHeld(Keys.D, moveAxisCmd);
            keyboard.BindHeld(Keys.Right, moveAxisCmd);

            // Recompute on release (so axis returns to 0 properly)
            keyboard.BindRelease(Keys.A, moveAxisCmd);
            keyboard.BindRelease(Keys.Left, moveAxisCmd);
            keyboard.BindRelease(Keys.D, moveAxisCmd);
            keyboard.BindRelease(Keys.Right, moveAxisCmd);

            // ---- Vertical intent axis (Y) ----
            // Update while held
            keyboard.BindHeld(Keys.W, vertAxisCmd);
            keyboard.BindHeld(Keys.Up, vertAxisCmd);
            keyboard.BindHeld(Keys.S, vertAxisCmd);
            keyboard.BindHeld(Keys.Down, vertAxisCmd);

            // Recompute on release
            keyboard.BindRelease(Keys.W, vertAxisCmd);
            keyboard.BindRelease(Keys.Up, vertAxisCmd);
            keyboard.BindRelease(Keys.S, vertAxisCmd);
            keyboard.BindRelease(Keys.Down, vertAxisCmd);

            // ---- Jump (press + release) ----
            keyboard.BindPress(Keys.Space, jumpPressedCmd);
            keyboard.BindHeld(Keys.Space, jumpHeldCmd);

            keyboard.BindPress(Keys.Q, new QuitCommand(game1));
            keyboard.BindPress(Keys.R, new ResetCommand(game1));

            // Block cycling (t = previous, y = next)
            keyboard.BindPress(Keys.T, new CycleBlockCommand(-1));
            keyboard.BindPress(Keys.Y, new CycleBlockCommand(1));

            // Attack
            keyboard.BindPress(Keys.Z, new AttackCommand());
            keyboard.BindPress(Keys.N, new AttackCommand());

            // Damage
            keyboard.BindPress(Keys.E, new DamageCommand());
            keyboard.BindPress(Keys.D1, new AttackCommand());
            keyboard.BindPress(Keys.D2, new ShootFireballCommand());
            keyboard.BindHeld(Keys.D3, new HealCommand());


        }
    }
}
