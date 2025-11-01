using System.Collections;
using System.Collections.Generic;
using System.IO;
using DIALOGUE;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public static Transition instance { get; private set; }
    [SerializeField] private InputReader inputReader;
    // [SerializeField] private TextAsset fileToRead;

    void Awake()
    {
        instance = this;
    }

    public void LoadLevel(string levelName, TextAsset fileToRead = null)
    {
        StartCoroutine(TransitionToNextLevel(levelName, fileToRead));
    }
    public void LoadNextScene(string levelName, bool playCutscene = false, TextAsset fileToRead = null)
    {
        StartCoroutine(TransitionToNextScene(levelName, playCutscene, fileToRead));
    }



    private IEnumerator TransitionToNextLevel(string levelName, TextAsset fileToRead = null)
    {
        inputReader.Disable();

        anim.SetTrigger("Start");

        int levelIndex = LevelProgressManager.FindLevelIndex(levelName);
        string nextLevel = LevelProgressManager.GetNextLevelName(levelIndex);

        if (fileToRead != null)
        {
            yield return StartCoroutine(StartConversation(fileToRead));
            yield return new WaitForSeconds(1f);
        }
        else
            yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextLevel);

    }

    private IEnumerator TransitionToNextScene(string levelName, bool playCutscene = false, TextAsset fileToRead = null)
    {
        inputReader.Disable();

        anim.SetTrigger("Start");

        if (playCutscene && fileToRead != null)
        {
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(StartConversation(fileToRead));
            yield return new WaitForSeconds(1f);
        }
        else
            yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(levelName);

    }

    private IEnumerator StartConversation(TextAsset fileToRead)
    {
        // set the player control to the general so they can go to next line
        inputReader.SetGeneral();
        string fullPath = AssetDatabase.GetAssetPath(fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10);
        string filePath = Path.ChangeExtension(relativePath, null);

        LoadFile(filePath);
        while (GeneralManager.instance.isRunningDialogue)
        {
            yield return null;
        }


        // once dialogue is done, set the player control back
        inputReader.SetPlayerMovement();
    }

    public void LoadFile(string filePath)
    {
        List<string> lines = new List<string>();

        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTxtAsset(file);
        }
        catch
        {
            Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist");
            return;
        }

        DialogueSystem.instance.mainCanv.sortingLayerName = "UI";
        DialogueSystem.instance.mainCanv.sortingOrder = 100;
        DialogueSystem.instance.Say(lines, filePath);
    }
}
