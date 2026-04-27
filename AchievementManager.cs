public static class AchievementManager
{
    public class Achievement
    {
        public string Name;
        public string Description;
        public bool Unlocked;

        public Achievement(string name, string description)
        {
            Name = name;
            Description = description;
            Unlocked = false;
        }
    }

    public static readonly Achievement Monopoly       = new Achievement("Monopoly",       "Collect 100 Geo");
    public static readonly Achievement Winner         = new Achievement("Winner",         "Beat the game");
    public static readonly Achievement WhatsThis      = new Achievement("What's this",    "???");
    public static readonly Achievement Robinhood      = new Achievement("Robinhood",      "Beat the game without taking a hit");
    public static readonly Achievement TrueDifficulty = new Achievement("True Difficulty", "Beat the game on hard mode");

    public static readonly Achievement[] All = { Monopoly, Winner, WhatsThis, Robinhood, TrueDifficulty };

    public static void Unlock(Achievement achievement)
    {
        achievement.Unlocked = true;
    }
}