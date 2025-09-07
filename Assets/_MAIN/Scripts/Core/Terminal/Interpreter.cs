using UnityEngine;
using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using COMMANDS;
using Unity.VisualScripting;

public class Interpreter : MonoBehaviour
{
    [SerializeField] private UserFuncManager userFuncManager;
    [SerializeField] private Player player;


    [DoNotSerialize] private PythonOutputRedirector pythonOutputRedirector;
    [DoNotSerialize] private Queue<IEnumerator> commandQueue = new();
    [DoNotSerialize] private bool isRunning = false;
    [DoNotSerialize] dynamic np;
    [DoNotSerialize] private PyModule scope;
    private GameStateManager gameState => GameStateManager.instance;
    void Start()
    {

        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded_python/python313.dll";
        PythonEngine.Initialize();



        using (Py.GIL())
        {
            scope = Py.CreateScope();
            pythonOutputRedirector = new PythonOutputRedirector();

            dynamic sys = Py.Import("sys");
            dynamic io = Py.Import("io");
            dynamic output = io.StringIO();
            sys.stdout = PyObject.FromManagedObject(pythonOutputRedirector);
            sys.stderr = PyObject.FromManagedObject(pythonOutputRedirector);

            try
            {
                scope.Set("ChangeText", new Action<string>(this.ChangeText));
                scope.Set("moveRight", new Action(MoveRight));
                scope.Set("moveLeft", new Action(MoveLeft));
                scope.Set("jump", new Action(Jump));

            }
            catch (Exception e)
            {
                print(e);
            }
        }

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
    public void ClearOutput() => pythonOutputRedirector.Clear();
    public string GetOutput() => pythonOutputRedirector.GetOutput();


    private IEnumerator RunCommand(IEnumerator command)
    {
        isRunning = true;
        yield return new WaitUntil(() => !player.isMoving);

        yield return StartCoroutine(command);
        isRunning = false;

    }


    public void ExecuteCode(string input)
    {
        if (gameState.isPaused || gameState.isRunningDialogue || isRunning || player.isMoving)
        {
            Debug.Log("Interpreter is busy, skipping code.");
            return;
        }
        if (!PythonEngine.IsInitialized) return;

        using (Py.GIL())
        {
            try
            {
                scope.Exec(input);


            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

    }
    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown();
        }
    }


    // add pre defined functions here
    private void Greet() => Debug.Log("Hello from C#");
    private void ChangeText(string text) => Debug.Log(text);
    private void MoveRight() => commandQueue.Enqueue(player.PlayerMoveRight());
    private void MoveLeft() => commandQueue.Enqueue(player.PlayerMoveLeft());
    private void Jump() => commandQueue.Enqueue(player.Jump());


}

