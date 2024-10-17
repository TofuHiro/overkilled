[System.Serializable]
public class GameData
{
    public SerializableDictionary<Level, LevelInfo> LevelInfos;

    //Constructor for default values for new save file
    public GameData()
    {
        LevelInfos = new SerializableDictionary<Level, LevelInfo>
        {
            { Level.Jan_1, new LevelInfo(Level.Jan_1, true, 0, Grade.NoStars) }
        };
    }
}
