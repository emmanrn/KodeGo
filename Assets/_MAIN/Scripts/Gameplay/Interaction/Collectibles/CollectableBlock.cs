using System;
using UnityEngine;

public class CollectableBlock : MonoBehaviour
{
    public string levelName;
    private bool collected;

    public event Action OnCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
            Collect();
    }

    private void Collect()
    {
        collected = true;
        LevelProgressManager.AddCollectedBlock(levelName);
        OnCollected?.Invoke();

        gameObject.SetActive(false);
    }


}
