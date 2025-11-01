using MAIN_GAME;
using TMPro;
using UnityEngine;

public class CodeBlockCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpro;
    private int totalBlocks = 3;
    public CollectableBlock[] blocks;

    void OnEnable()
    {
        foreach (var block in blocks)
            block.OnCollected += UpdateCounter;

        UpdateCounter();
    }

    void Awake()
    {
        this.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        foreach (var block in blocks)
            block.OnCollected -= UpdateCounter;
    }


    private void UpdateCounter()
    {
        int collected = LevelProgressManager.runtime[GameManager.instance.LEVEL_NAME].collectedBlocks;

        tmpro.text = $"{collected} / {totalBlocks} Collected";
    }
}
