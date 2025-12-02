using System;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN_GAME
{
    [System.Serializable]
    public class Game_LevelData
    {
        public string levelName;
        public bool unlocked = false;
        public bool completed = false;
        public int collectedBlocks;
        public bool quizPassed;
        public bool secretFound;
        public string skinUnlocked;
        public int deathCount;
        public string title;

        public bool secretSkinCollected;
        public int practiceTerminalsSolved;
        public int debugTerminalsSolved;
        public int finalTerminalsSolved;

        public int TotalTerminalSolved => practiceTerminalsSolved + debugTerminalsSolved + finalTerminalsSolved;

        public float explorationPercent;

        // This will not serialize directly
        [NonSerialized]
        public HashSet<string> visitedRooms = new HashSet<string>();

        // Temporary list for serialization
        [SerializeField] private List<string> visitedRoomsList = new List<string>();

        public Vector3 checkpoint;
        public bool hasCheckpoint;

        public float completionPrecent;

        // Call this before saving
        public void PrepareForSave()
        {
            visitedRoomsList = new List<string>(visitedRooms);
        }

        // Call this after loading
        public void RestoreAfterLoad()
        {
            visitedRooms = new HashSet<string>(visitedRoomsList);
        }

    }

}