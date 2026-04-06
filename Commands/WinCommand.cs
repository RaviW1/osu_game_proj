using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class WinCommand : ICommand
    {
        private GameScene scene;
        public WinCommand(GameScene scene) { this.scene = scene; }
        public void Execute(Player player, GameTime gameTime) { scene.TriggerWin(); }
    }
}