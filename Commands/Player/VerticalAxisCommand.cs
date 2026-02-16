using Microsoft.Xna.Framework.Input;
using System;

public sealed class VerticalAxisCommand : ICommand
{
    private readonly Keys[] _upKeys;
    private readonly Keys[] _downKeys;

    public int AxisY { get; private set; } // -1,0,+1

    public VerticalAxisCommand(Keys[] upKeys, Keys[] downKeys)
    {
        _upKeys = upKeys ?? Array.Empty<Keys>();
        _downKeys = downKeys ?? Array.Empty<Keys>();
    }

    public void Execute()
    {
        var s = Keyboard.GetState();
        bool up = AnyDown(s, _upKeys);
        bool down = AnyDown(s, _downKeys);

        AxisY = (up == down) ? 0 : (up ? 1 : -1);

        // Later:
        // player.SetVerticalIntent(AxisY);
    }

    private static bool AnyDown(KeyboardState s, Keys[] keys)
    {
        foreach (var k in keys) if (s.IsKeyDown(k)) return true;
        return false;
    }
}
