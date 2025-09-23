using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class TestNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextAsset fileToRead;
    private GameManager gameState;

    void Awake()
    {
        gameState = GameManager.instance;
    }

    public bool isInteractable() => !gameState.isRunningDialogue;
    public void Interact()
    {
        Debug.Log("npc");
        if (gameState.isPaused && !gameState.isRunningDialogue)
            return;

        if (!gameState.isRunningDialogue && isInteractable())
            StartCoroutine(StartConversation());

    }

    private IEnumerator StartConversation()
    {
        // set the player control to the general so they can go to next line
        inputReader.SetGeneral();

        List<string> lines = FileManager.ReadTxtAsset(fileToRead);
        yield return DialogueSystem.instance.Say(lines);

        // once dialogue is done, set the player control back
        inputReader.SetPlayerMovement();
    }


}