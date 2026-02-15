using Microsoft.Xna.Framework.Input;
using System;

public sealed class MovementAxisCommand : ICommand
{
    private readonly Keys[] _leftKeys;
    private readonly Keys[] _rightKeys;

    public int AxisX { get; private set; } // -1,0,+1

    public MovementAxisCommand(Keys[] leftKeys, Keys[] rightKeys)
    {
        _leftKeys = leftKeys ?? Array.Empty<Keys>();
        _rightKeys = rightKeys ?? Array.Empty<Keys>();
    }

    public void Execute(Player player)
    {
        var s = Keyboard.GetState();
        bool left = AnyDown(s, _leftKeys);
        bool right = AnyDown(s, _rightKeys);

        AxisX = (left == right) ? 0 : (left ? -1 : 1);

        // Later:
        player.Walk(AxisX);
    }

    private static bool AnyDown(KeyboardState s, Keys[] keys)
    {
        foreach (var k in keys) if (s.IsKeyDown(k)) return true;
        return false;
    }
}
