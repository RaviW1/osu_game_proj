namespace osu_game_proj
{
    /// <summary>
    /// Global difficulty switch set on the menu before launching GameScene.
    /// Read by enemy constructors and combat code to scale stats / rewards.
    /// </summary>
    public static class Difficulty
    {
        public static bool IsHardMode { get; set; } = false;

        // Soul granted per successful melee hit on an enemy.
        public static int SoulPerMeleeHit => IsHardMode ? 5 : 10;

        // Multiplier applied to every enemy's base HP at construction time.
        public static int HpMultiplier => IsHardMode ? 2 : 1;
    }
}
