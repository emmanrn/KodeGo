
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

    public Vector3 checkpoint;
    public bool hasCheckpoint;

    public float completionPrecent;
}
