using System.Collections;
using System.IO;
using MAIN_GAME;
using UnityEditor;
using UnityEngine;

public class TestNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextAsset fileToRead;

    void Awake()
    {
    }

    public bool isInteractable() => !GeneralManager.instance.isRunningDialogue;
    public void Interact()
    {
        Debug.Log("npc");
        if (GeneralManager.instance.isPaused && !GeneralManager.instance.isRunningDialogue)
            return;

        if (isInteractable())
            StartCoroutine(StartConversation());

    }

    private IEnumerator StartConversation()
    {
        // set the player control to the general so they can go to next line
        inputReader.SetGeneral();
        string fullPath = AssetDatabase.GetAssetPath(fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10);
        string filePath = Path.ChangeExtension(relativePath, null);

        // List<string> lines = FileManager.ReadTxtAsset(fileToRead);
        // yield return DialogueSystem.instance.Say(lines);
        GameManager.instance.LoadFile(filePath);
        while (GeneralManager.instance.isRunningDialogue)
            yield return null;

        // once dialogue is done, set the player control back
        inputReader.SetPlayerMovement();
    }


}