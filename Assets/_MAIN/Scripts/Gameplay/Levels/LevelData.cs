
using System.Collections.Generic;
using UnityEngine;

public class LevelData
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
    public HashSet<string> solvedPracticeTerminals = new HashSet<string>();
    public HashSet<string> solvedDebugTerminals = new HashSet<string>();
    public HashSet<string> solvedFinalTerminals = new HashSet<string>();
    public int practiceTerminalsSolved => solvedPracticeTerminals.Count;
    public int debugTerminalsSolved => solvedDebugTerminals.Count;
    public int finalTerminalsSolved => solvedFinalTerminals.Count;

    public int TotalTerminalSolved => practiceTerminalsSolved + debugTerminalsSolved + finalTerminalsSolved;

    public float explorationPercent;
    public HashSet<string> visitedRooms = new HashSet<string>();

    public Vector3 checkpoint;
    public bool hasCheckpoint;

    public float completionPrecent;
    public int failedAttempts;
}
