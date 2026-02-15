using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class KeyboardController : IController
{
    private readonly Dictionary<Keys, ICommand> onPress = new();
    private readonly Dictionary<Keys, ICommand> whileHeld = new();
    private readonly Dictionary<Keys, ICommand> onRelease = new();

    private readonly Dictionary<Keys, ICommand> mappings = new();
    private KeyboardState previousState;

    public void BindPress(Keys key, ICommand cmd) => onPress[key] = cmd;
    public void BindHeld(Keys key, ICommand cmd) => whileHeld[key] = cmd;
    public void BindRelease(Keys key, ICommand cmd) => onRelease[key] = cmd;
    public void RegisterCommand(Keys key, ICommand command)
    {
        mappings[key] = command;
    }

    //public void Update()
    //{
    //    KeyboardState current = Keyboard.GetState();
    //
    //    foreach (var pair in mappings)
    //    {
    //        Keys key = pair.Key;
    //
    //        if (current.IsKeyDown(key) && previousState.IsKeyUp(key))
    //        {
    //            pair.Value.Execute();
    //        }
    //    }
    //
    //    previousState = current;
    //}

    public List<ICommand> GetCommands()
    {
        var activeCommands = new List<ICommand>();
        KeyboardState current = Keyboard.GetState();

        foreach (var pair in onPress)
        {
            Keys key = pair.Key;
            if (current.IsKeyDown(key) && previousState.IsKeyUp(key))
            {
                activeCommands.Add(pair.Value);
            }
        }
        foreach (var (key, cmd) in whileHeld)
            if (current.IsKeyDown(key))
                activeCommands.Add(cmd);
        foreach (var (key, cmd) in onRelease)
            if (current.IsKeyUp(key) && previousState.IsKeyDown(key))
                activeCommands.Add(cmd);

        previousState = current;
        return activeCommands;
    }
}

