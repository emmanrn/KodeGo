using System.Collections;
using System.Collections.Generic;
using System.IO;
using DIALOGUE;
using MAIN_GAME;
using UnityEditor;
using UnityEngine;

public class Quiz : MonoBehaviour, IInteractable
{
    private const int MAX_WRONG_ATTEMPTS = 5;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextAsset fileToRead;
    [SerializeField] private GameObject door;
    [SerializeField] private string[] hints;

    [SerializeField] private string quizId;
    private string levelName => GameManager.instance.LEVEL_NAME;
    private string varKey;
    public bool isCorrect = false;
    private int attempts;
    private int prevHintIndex = -1;



    void Start()
    {
        attempts = 0;
        varKey = $"{levelName}.Quiz_{quizId}";

        if (VariableStore.TryGetValue(varKey, out var val))
        {
            isCorrect = (bool)val;
        }
        else
        {
            isCorrect = false;
            VariableStore.TrySetValue(varKey, false);
        }


        if (isCorrect)
        {
            if (door != null)
            {
                if (door.TryGetComponent(out CollectableBlock block))
                {
                    block.isCollectable = true;
                    Color alpha = Color.yellow;
                    block.GetComponent<SpriteRenderer>().color = alpha;
                    return;
                }

                if (door.TryGetComponent(out PlatformEffector2D platform) && door.TryGetComponent(out OneWayPlatform oneWayPlatform))
                {
                    platform.enabled = true;
                    oneWayPlatform.enabled = true;
                    door.GetComponent<SpriteRenderer>().color = new Color(0f, 0.30f, 1f);
                    return;
                }

                door.SetActive(false);

            }
        }
    }

    public bool isInteractable() => !GeneralManager.instance.isRunningDialogue;
    public void Interact()
    {
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
        LoadFile(filePath);
        while (GeneralManager.instance.isRunningDialogue)
        {
            CheckAnswer();
            yield return null;
        }
        if (!isCorrect)
        {
            attempts++;
            bool thresholdReached = (attempts % MAX_WRONG_ATTEMPTS == 0) ? true : false;
            if (thresholdReached)
                ShowHint();
        }


        // once dialogue is done, set the player control back
        inputReader.SetPlayerMovement();
    }

    private void CheckAnswer()
    {
        if (VariableStore.TryGetValue(varKey, out var val))
            isCorrect = (bool)val;

        if (isCorrect)
        {
            LevelProgressManager.SetQuizPassed(levelName);
            if (door != null)
            {
                if (door.TryGetComponent(out CollectableBlock block))
                {
                    block.isCollectable = true;
                    Color alpha = Color.yellow;
                    block.GetComponent<SpriteRenderer>().color = alpha;
                    return;
                }

                if (door.TryGetComponent(out PlatformEffector2D platform) && door.TryGetComponent(out OneWayPlatform oneWayPlatform))
                {
                    platform.enabled = true;
                    oneWayPlatform.enabled = true;
                    door.GetComponent<SpriteRenderer>().color = new Color(0f, 0.30f, 1f);
                    return;
                }

                door.SetActive(false);
            }
        }


    }

    private void ShowHint()
    {
        if (hints == null || hints.Length == 0)
            return;

        int randomHintIndex;

        // If there’s only one hint, just show it.
        if (hints.Length == 1)
        {
            randomHintIndex = 0;
        }
        else
        {
            do
            {
                randomHintIndex = Random.Range(0, hints.Length);
            }
            while (randomHintIndex == prevHintIndex);
        }

        prevHintIndex = randomHintIndex;
        PopupMenuManager.instance.ShowHintPopup(hints[randomHintIndex]);
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

        DialogueSystem.instance.Say(lines, filePath);
    }
}
