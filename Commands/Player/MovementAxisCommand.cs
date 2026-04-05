using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public sealed class MovementAxisCommand : ICommand
{
    private readonly Keys[] _leftKeys;
    private readonly Keys[] _rightKeys;
    private int _lastFrame = -1;  // ADD THIS

    public int AxisX { get; private set; }

    public MovementAxisCommand(Keys[] leftKeys, Keys[] rightKeys)
    {
        _leftKeys = leftKeys ?? Array.Empty<Keys>();
        _rightKeys = rightKeys ?? Array.Empty<Keys>();
    }

    public void Execute(Player player, GameTime gameTime)
    {
        // Only execute once per frame regardless of how many keys triggered it
        int currentFrame = (int)gameTime.TotalGameTime.TotalMilliseconds;
        if (currentFrame == _lastFrame) return;
        _lastFrame = currentFrame;

        var s = Keyboard.GetState();
        bool left = AnyDown(s, _leftKeys);
        bool right = AnyDown(s, _rightKeys);
        AxisX = (left == right) ? 0 : (left ? -1 : 1);
        player.Walk(AxisX);
    }

    private static bool AnyDown(KeyboardState s, Keys[] keys)
    {
        foreach (var k in keys) if (s.IsKeyDown(k)) return true;
        return false;
    }
}
