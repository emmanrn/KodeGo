using UnityEngine;
using Python.Runtime;
using System;
using Unity.VisualScripting;
using TMPro;

public class Interpreter : MonoBehaviour
{


    [DoNotSerialize] private PythonOutputRedirector pythonOutputRedirector;
    [DoNotSerialize] private bool isRunning = false;
    [DoNotSerialize] dynamic np;
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI outputCode;
    [DoNotSerialize] private PyModule scope;

    private GameStateManager gameState;

    void Awake()
    {
        gameState = GameStateManager.instance;
    }
    void Start()
    {

        Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/embedded_python/python313.dll";
        PythonEngine.Initialize();



        using (Py.GIL())
        {
            scope = Py.CreateScope();
            pythonOutputRedirector = new PythonOutputRedirector();

            dynamic sys = Py.Import("sys");
            sys.stdout = PyObject.FromManagedObject(pythonOutputRedirector);
            sys.stderr = PyObject.FromManagedObject(pythonOutputRedirector);

        }


    }

    public void GetCode()
    {
        TextMeshProUGUI[] codes = content.GetComponentsInChildren<TextMeshProUGUI>();
        string[] lines = new string[codes.Length];

        for (int i = 0; i < lines.Length; i++)
            lines[i] = codes[i].text;

        string inputCode = string.Join("\n", lines);
        ExecuteCode(inputCode);
    }

    public void ExecuteCode(string input)
    {
        // if (gameState.isPaused || gameState.isRunningDialogue)
        //     return;

        if (!PythonEngine.IsInitialized) return;

        using (Py.GIL())
        {
            try
            {
                scope.Exec(input);

                string pyOut = pythonOutputRedirector.GetOutput();
                if (!string.IsNullOrWhiteSpace(pyOut))
                    Debug.Log(pyOut);

                outputCode.text = pyOut;



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



}

