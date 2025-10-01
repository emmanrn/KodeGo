using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private GameObject[] lifeIcons; // Assign 3 heart GameObjects in inspector

    private void Start()
    {
        if (GameManager.instance.Player is PlayerLife playerLife)
        {
            playerLife.OnLivesChanged += UpdateLivesUI;
            UpdateLivesUI(playerLife.CurrentLives); // initial sync
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance.Player is PlayerLife playerLife)
        {
            playerLife.OnLivesChanged -= UpdateLivesUI;
        }
    }

    private void UpdateLivesUI(int livesRemaining)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Hearts disappear as lives go down
            lifeIcons[i].SetActive(i < livesRemaining);
        }
    }
}
