using DIALOGUE;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private GameObject pauseMenu;
    public bool isPaused { get; private set; } = false;
    public bool isRunningDialogue => DialogueSystem.instance.isRunningConversation;
    public static GameStateManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void OnEnable()
    {
        input.PauseEvent += HandlePause;
        input.ResumeEvent += HandleResume;
    }
    private void OnDisable()
    {
        input.PauseEvent -= HandlePause;
        input.ResumeEvent -= HandleResume;
    }

    public void HandlePause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
    }

    public void HandleResume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
    }
}
