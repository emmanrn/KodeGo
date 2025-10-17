using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData_SO : ScriptableObject
{
    public string levelName;
    public int totalBlocks = 3;
    public Sprite previewImg;

}
