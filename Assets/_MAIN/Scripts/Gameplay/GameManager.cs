using DIALOGUE;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private GameObject pauseMenu;
    public bool isPaused { get; private set; } = false;
    public bool isRunningDialogue => DialogueSystem.instance.isRunningConversation;
    public static GameManager instance { get; set; }
    public IDamageable Player { get; private set; }

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

    // for no this is what we do for the player losing life in the terminal, we just use a singleton of the gameManger
    // but we are not directly making the game manager do the taking damage, the one that does the dmg is still the player.
    // we are just doing this because it's serving as a communicator to the terminal
    // so we are registering the player here which the player has a IDamageable interface.
    // after the player is registered then the terminal can communinicate to the player to take dmg.
    // For now, this is the solution for this.
    // It's still kind off coupled, BUT not very tightly coupled than making the GameManager hndle the taking the dmg of the player from the terminal
    // The gamemanager is just serving as a communicator for the two.
    public void RegisterPlayer(IDamageable player) => Player = player;


}
