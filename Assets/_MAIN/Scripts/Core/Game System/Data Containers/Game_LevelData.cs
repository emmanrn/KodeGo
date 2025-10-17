
namespace MAIN_GAME
{
    [System.Serializable]
    public class Game_LevelData
    {
        public string levelName;
        public bool unlocked;
        public bool completed;
        public int collectedBlocks;
        public bool quizPassed;
        public bool secretFound;
        public string skinUnlocked;

        public float completionPrecent;
    }

}