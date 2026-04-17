public class HitstopManager
{
    private int _framesRemaining;

    public bool IsActive => _framesRemaining > 0;

    /// <summary>
    /// Freeze gameplay for the given number of frames.
    /// Calling Trigger while already active takes the max of the two durations
    /// so a rapid combo never cuts a hitstop short.
    /// </summary>
    public void Trigger(int frames)
    {
        if (frames > _framesRemaining)
            _framesRemaining = frames;
    }

    /// <summary>
    /// Call once per frame in GameScene.Update, before the early-return check.
    /// </summary>
    public void Update()
    {
        if (_framesRemaining > 0)
            _framesRemaining--;
    }
}