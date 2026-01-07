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
        // ✅ Serializable terminal sets
        [SerializeField] private List<string> solvedPracticeTerminals = new();
        [SerializeField] private List<string> solvedDebugTerminals = new();
        [SerializeField] private List<string> solvedFinalTerminals = new();

        // Runtime versions
        [NonSerialized] public HashSet<string> practiceSet = new();
        [NonSerialized] public HashSet<string> debugSet = new();
        [NonSerialized] public HashSet<string> finalSet = new();

        // Derived counts
        public int practiceTerminalsSolved => practiceSet.Count;
        public int debugTerminalsSolved => debugSet.Count;
        public int finalTerminalsSolved => finalSet.Count;
        public int TotalTerminalSolved =>
            practiceTerminalsSolved + debugTerminalsSolved + finalTerminalsSolved;

        public float explorationPercent;

        // This will not serialize directly
        [NonSerialized]
        public HashSet<string> visitedRooms = new HashSet<string>();

        // Temporary list for serialization
        [SerializeField] private List<string> visitedRoomsList = new List<string>();

        public Vector3 checkpoint;
        public bool hasCheckpoint;

        public float completionPrecent;
        public int failedAttempts;

        // Call this before saving
        public void PrepareForSave()
        {
            solvedPracticeTerminals = new List<string>(practiceSet);
            solvedDebugTerminals = new List<string>(debugSet);
            solvedFinalTerminals = new List<string>(finalSet);

            visitedRoomsList = new List<string>(visitedRooms);
        }

        // Call this after loading
        public void RestoreAfterLoad()
        {
            practiceSet = new HashSet<string>(solvedPracticeTerminals);
            debugSet = new HashSet<string>(solvedDebugTerminals);
            finalSet = new HashSet<string>(solvedFinalTerminals);

            visitedRooms = new HashSet<string>(visitedRoomsList);
        }

    }

}