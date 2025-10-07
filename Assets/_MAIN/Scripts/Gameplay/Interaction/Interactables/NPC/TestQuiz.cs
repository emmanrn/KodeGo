using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

public class TestQuiz : MonoBehaviour, IInteractable
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextAsset fileToRead;

    void Awake()
    {
    }

    public bool isInteractable() => !GameManager.instance.isRunningDialogue;
    public void Interact()
    {
        Debug.Log("npc");
        if (GameManager.instance.isPaused && !GameManager.instance.isRunningDialogue)
            return;

        if (isInteractable())
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
