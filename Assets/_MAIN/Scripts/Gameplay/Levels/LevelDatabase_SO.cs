using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Database")]
public class LevelDatabase_SO : ScriptableObject
{
    public List<LevelData_SO> levels;
}
