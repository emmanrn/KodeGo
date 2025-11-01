using UnityEngine;

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
        public int deathCount;
        public string title;

        public Vector3 checkpoint;
        public bool hasCheckpoint;

        public float completionPrecent;
    }

}