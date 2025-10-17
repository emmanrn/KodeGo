using System.Collections;
using System.Collections.Generic;
using MAIN_GAME;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public static Transition instance { get; private set; }
    [SerializeField] private InputReader inputReader;

    void Awake()
    {
        instance = this;
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(TransitionToNextLevel(levelName));
    }

    private IEnumerator TransitionToNextLevel(string levelName)
    {
        inputReader.Disable();

        anim.SetTrigger("Start");

        int levelIndex = LevelProgressManager.FindLevelIndex(levelName);
        string nextLevel = LevelProgressManager.GetNextLevelName(levelIndex);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextLevel);

    }
}
