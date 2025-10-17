using MAIN_GAME;
using UnityEngine;
public class CollectedBlocks : MonoBehaviour
{
    [SerializeField] private GameObject[] collectibleBlocks;
    private string levelName => GameManager.instance.LEVEL_NAME;
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
