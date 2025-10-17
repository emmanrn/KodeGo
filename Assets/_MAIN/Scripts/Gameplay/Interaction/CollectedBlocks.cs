using MAIN_GAME;
using UnityEngine;
public class CollectedBlocks : MonoBehaviour
{
    [SerializeField] private GameObject[] collectibleBlocks;
    [SerializeField] private string levelName = "Level1";
    void Start()
    {

        for (int i = 0; i < collectibleBlocks.Length; i++)
        {
            GameObject block = collectibleBlocks[i];
            CollectableBlock collectible = block.GetComponent<CollectableBlock>();
            collectible.levelName = levelName;
            bool collected = i < LevelProgressManager.runtime[levelName].collectedBlocks;
            block.SetActive(!collected);
        }
    }



}
