using UnityEngine;
using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    [SerializeField] private UserFuncManager userFuncManager;
    Player player;
    private Queue<IEnumerator> commandQueue = new();
    private bool isRunning = false;
    dynamic np;
    void Start()
    {
        player = GetComponent<Player>();

        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded_python/python313.dll";
        PythonEngine.Initialize();


        try
        {
            dynamic sys = PyModule.Import("sys");
            print("Python version: " + sys.version);

            dynamic math = PyModule.Import("math");
            print("math.pi: " + math.pi);

            dynamic os = PyModule.Import("os");
            print("Current directory: " + os.getcwd());


        }
        catch (Exception e)
        {
            print(e);
            print(e.StackTrace);
        }



    }

    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isRunning && commandQueue.Count > 0)
        {
            StartCoroutine(RunCommand(commandQueue.Dequeue()));
        }
    }

    private IEnumerator RunCommand(IEnumerator command)
    {
        isRunning = true;
        yield return StartCoroutine(command); // waits until PlayerMoveRight/Left finishes
        isRunning = false;
    }

    private void EnqueueCommand(IEnumerator command)
    {
        commandQueue.Enqueue(command);
    }

    public void Greet()
    {
        Debug.Log("Hello from C#!");
    }

    public void ChangeText(string sampleText)
    {

        Debug.Log(sampleText);


    }

    private void MoveRight()
    {
        EnqueueCommand(player.PlayerMoveRight());
    }
    private void MoveLeft()
    {
        EnqueueCommand(player.PlayerMoveLeft());
    }





    public void ExecuteCode(string input)
    {
        string code = input;
        Debug.Log(code);

        if (PythonEngine.IsInitialized)
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                dynamic io = Py.Import("io");
                dynamic output = io.StringIO();
                sys.stdout = output;

                using (var scope = Py.CreateScope())
                {
                    try
                    {
                        scope.Set("ChangeText", new Action<string>(this.ChangeText));
                        scope.Set("moveRight", new Action(MoveRight));
                        scope.Set("moveLeft", new Action(MoveLeft));
                        scope.Exec(code);
                        string res = output.getvalue().ToString();
                        sys.stdout = sys.__stdout__;
                        userFuncManager.DisplayResult(res);



                    }
                    catch (Exception e)
                    {
                        print(e);
                    }
                }
            }
        }

    }


}

