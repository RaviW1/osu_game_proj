using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Windows.Input;

public class KeyboardController : IController
{
    private readonly Dictionary<Keys, ICommand> mappings = new();
    private KeyboardState previousState;

    public void RegisterCommand(Keys key, ICommand command)
    {
        mappings[key] = command;
    }

    public void Update()
    {
        KeyboardState current = Keyboard.GetState();

        foreach (var pair in mappings)
        {
            Keys key = pair.Key;

            if (current.IsKeyDown(key) && previousState.IsKeyUp(key))
            {
                pair.Value.Execute();
            }
        }

        previousState = current;
    }
}
