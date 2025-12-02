using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Database")]
public class LevelDatabase_SO : ScriptableObject
{
    public LevelData_SO[] levels;
    // Optional helper method to find a level by name
    public LevelData_SO GetLevel(string levelName)
    {
        foreach (var level in levels)
        {
            if (level.levelName == levelName)
                return level;
        }
        return null;
    }
}
