using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LevelProgressManager
{
    public static LevelDatabase_SO levels;
    public static Dictionary<string, LevelData> runtime = new Dictionary<string, LevelData>();

    public static void Initialize(LevelDatabase_SO levelDB)
    {
        foreach (var level in levelDB.levels)
        {
            if (!runtime.ContainsKey(level.levelName))
            {
                runtime.Add(level.levelName, new LevelData
                {
                    levelName = level.levelName
                });
            }
        }

        if (levelDB.levels.Length > 0)
            runtime[levelDB.levels[0].levelName].unlocked = true;
        levels = levelDB;
    }



    public static LevelData GetLevel(string name)
    {
        if (!runtime.ContainsKey(name))
            runtime[name] = new LevelData { levelName = name };

        return runtime[name];

    }

    public static void SetLevel(string name, LevelData data)
    {
        runtime[name] = data;
    }

    public static void AddCollectedBlock(string levelName)
    {
        var level = GetLevel(levelName);
        level.collectedBlocks++;
        RecalculateCompletion(level);
    }

    public static void SetQuizPassed(string levelName)
    {
        var level = GetLevel(levelName);
        level.quizPassed = true;
        RecalculateCompletion(level);
    }

    public static void SetSkinUnlocked(string levelName, string skinName)
    {
        var level = GetLevel(levelName);
        level.skinUnlocked = skinName;
        RecalculateCompletion(level);
    }

    public static void AddDeathCount(string levelName)
    {
        var level = GetLevel(levelName);
        level.deathCount++;
        RecalculateCompletion(level);
    }

    public static void SetPlayerLevelWin(string levelName)
    {
        var level = GetLevel(levelName);
        level.completed = true;

        if (levels == null || levels.levels == null || levels.levels.Length == 0)
        {
            return;
        }
        else
        {
        }

        int currentIndex = FindLevelIndex(levelName);

        if (currentIndex >= 0 && currentIndex < levels.levels.Length - 1)
        {
            var nextLevel = levels.levels[currentIndex + 1];
            if (runtime.TryGetValue(nextLevel.levelName, out var nextLevelData))
            {
                nextLevelData.unlocked = true;
            }
            else
            {
                runtime[nextLevel.levelName] = new LevelData
                {
                    levelName = nextLevel.levelName,
                    unlocked = true
                };
            }
        }
    }

    public static int FindLevelIndex(string levelName)
    {
        return Array.FindIndex(levels.levels, l => l.levelName == levelName);
    }

    public static string GetNextLevelName(int currentIndex)
    {
        return levels.levels[currentIndex + 1].name;
    }



    private static void RecalculateCompletion(LevelData level)
    {
        // Example: compute progress %
        float percent = 0f;
        if (level.quizPassed) percent += 0.3f;
        if (level.secretFound) percent += 0.2f;
        percent += Mathf.Clamp01(level.collectedBlocks / 3f) * 0.5f;
        level.completionPrecent = percent;
    }

    // BEWARE OF THIS
    public static void RemoveAllLevelData()
    {
        runtime.Clear();
    }


}
