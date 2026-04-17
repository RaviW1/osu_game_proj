public partial class GameScene
{
    private HitstopManager _hitstop = new HitstopManager();

    public void TriggerHitEffects(bool playerWasHit)
    {
        if (playerWasHit)
        {
            _hitstop.Trigger(4);
            _camera.Shake(6f, 14);
        }
        else
        {
            _hitstop.Trigger(6);
            _camera.Shake(3f, 10);
        }
    }
}