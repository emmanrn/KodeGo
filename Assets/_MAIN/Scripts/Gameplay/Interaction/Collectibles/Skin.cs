using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private CharacterSkin skin;
    private bool collected;

    void Start()
    {
        if (LevelProgressManager.runtime[levelName].skinUnlocked == skin.name)
        {
            collected = true;
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
            Collect();
    }

    private void Collect()
    {
        collected = true;
        LevelProgressManager.SetSkinUnlocked(levelName, skin.name);
        gameObject.SetActive(false);
    }
}
