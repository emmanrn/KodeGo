using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PythonExecutor : MonoBehaviour
{
    private Queue<IEnumerator> commandQueue = new();
    private bool isRunning = false;
    private GameStateManager gameState => GameStateManager.instance;
    [SerializeField] private Player player;

    void Start()
    {
        StartCoroutine(CommandProcessor());
    }

    private IEnumerator CommandProcessor()
    {
        while (true)
        {
            if (gameState.isPaused || isRunning || gameState.isRunningDialogue || commandQueue.Count == 0)
            {
                yield return null;
                continue;
            }
            yield return StartCoroutine(RunCommand(commandQueue.Dequeue()));
        }
    }
    private IEnumerator RunCommand(IEnumerator command)
    {
        isRunning = true;
        yield return new WaitUntil(() => !player.isMoving);

        yield return StartCoroutine(command);
        isRunning = false;

    }
}
