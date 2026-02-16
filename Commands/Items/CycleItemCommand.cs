using System;

public class CycleItemCommand : ICommand
{
    private readonly int direction;
    private readonly Action<int> cycleAction;

    /// <param name="direction">+1 for next item, -1 for previous item</param>
    /// <param name="cycleAction">Callback that performs the actual cycling</param>
    public CycleItemCommand(int direction, Action<int> cycleAction)
    {
        this.direction = direction;
        this.cycleAction = cycleAction;
    }

    public void Execute(Player player)
    {
        cycleAction?.Invoke(direction);
    }
}
