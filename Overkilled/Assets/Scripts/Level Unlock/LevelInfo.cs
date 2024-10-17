[System.Serializable]
public class LevelInfo
{
    public Level Level;
    public bool Unlocked;
    public int HighestScore;
    public Grade HighestGrade;

    /// <summary>
    /// Empty constructor for default level info values
    /// </summary>
    public LevelInfo()
    {
        Level = Level.None;
        Unlocked = false;
        HighestScore = 0;
        HighestGrade = Grade.NoStars;
    }

    /// <summary>
    /// Constructor to create a new LevelInfo
    /// </summary>
    /// <param name="level">The level this LevelInfo represents</param>
    /// <param name="unlocked">If this level is unlocked or not</param>
    /// <param name="highestScore">The highest score achieved on this level</param>
    /// <param name="highestGrade">The highest grade achieved on this level</param>
    public LevelInfo(Level level, bool unlocked, int highestScore, Grade highestGrade)
    {
        Level = level;
        Unlocked = unlocked;
        HighestScore = highestScore;
        HighestGrade = highestGrade;
    }
}