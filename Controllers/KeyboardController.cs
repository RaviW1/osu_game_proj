using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class KeyboardController : IController
{
    private readonly Dictionary<Keys, ICommand> onPress = new();
    private readonly Dictionary<Keys, ICommand> whileHeld = new();
    private readonly Dictionary<Keys, ICommand> onRelease = new();

    private KeyboardState previousState;

    public void BindPress(Keys key, ICommand cmd) => onPress[key] = cmd;
    public void BindHeld(Keys key, ICommand cmd) => whileHeld[key] = cmd;
    public void BindRelease(Keys key, ICommand cmd) => onRelease[key] = cmd;

    public void Update()
    {
        var current = Keyboard.GetState();

        foreach (var (key, cmd) in onPress)
            if (current.IsKeyDown(key) && previousState.IsKeyUp(key))
                cmd.Execute();

        foreach (var (key, cmd) in whileHeld)
            if (current.IsKeyDown(key))
                cmd.Execute();

        foreach (var (key, cmd) in onRelease)
            if (current.IsKeyUp(key) && previousState.IsKeyDown(key))
                cmd.Execute();

        previousState = current;
    }
}

