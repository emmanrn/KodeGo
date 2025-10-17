using TMPro;
using UnityEngine;

public class CodeBlockCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpro;
    [SerializeField] private string levelName = "Level1";
    private int totalBlocks = 3;

    public CollectableBlock[] blocks;

    void OnEnable()
    {
        foreach (var block in blocks)
            block.OnCollected += UpdateCounter;

        UpdateCounter();
    }

    void OnDisable()
    {
        foreach (var block in blocks)
            block.OnCollected -= UpdateCounter;
    }


    private void UpdateCounter()
    {
        int collected = LevelProgressManager.runtime[levelName].collectedBlocks;

        tmpro.text = $"{collected} / {totalBlocks} Collected";
    }
}
