using Microsoft.Xna.Framework;

public class MuteCommand : ICommand
{

    public MuteCommand()
    {
    }

    public void Execute(Player player, GameTime gameTime)
    {
        SoundManager.ToggleMusic();
    }
}
