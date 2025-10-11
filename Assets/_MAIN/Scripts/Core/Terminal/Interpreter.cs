using UnityEngine;
using Python.Runtime;
using System;
using Unity.VisualScripting;
using TMPro;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

public class Interpreter : MonoBehaviour
{
    [DoNotSerialize] private PythonOutputRedirector pythonOutputRedirector;
    [DoNotSerialize] dynamic np;
    [DoNotSerialize] private PyModule scope;


    void Awake()
    {

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



    public string ExecuteCode(string input)
    {


        if (!PythonEngine.IsInitialized) return null;

        using (Py.GIL())
        {
            try
            {
                pythonOutputRedirector.Clear();
                scope.Exec(input);


                string pyOut = pythonOutputRedirector.GetOutput();


                if (string.IsNullOrWhiteSpace(pyOut))
                    Debug.Log(pyOut);


                return pyOut;



            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return e.ToString();
            }
        }



    }

    // public void OnApplicationQuit()
    // {
    //     if (PythonEngine.IsInitialized)
    //     {
    //         PythonEngine.Shutdown();
    //     }
    // }

    void OnDestroy()
    {
        if (PythonEngine.IsInitialized)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    using (Py.GIL())
                    {
                        scope?.Dispose();
                        scope = null;
                    }

                    // Clean shutdown on background thread
                    PythonEngine.Shutdown();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Python shutdown error: {e}");
                }
            });
        }
    }
}

