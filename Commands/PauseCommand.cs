using Microsoft.Xna.Framework;

namespace osu_game_proj
{
    public class PauseCommand : ICommand
    {
        private GameScene scene;
        public PauseCommand(GameScene scene) { this.scene = scene; }
        public void Execute(Player player, GameTime gameTime) { scene.TogglePause(); }
    }
}