using UnityEngine;
using Python.Runtime;
using System;
using Unity.VisualScripting;

public partial class Interpreter : MonoBehaviour
{
    [DoNotSerialize] private PythonOutputRedirector pythonOutputRedirector;
    [DoNotSerialize] dynamic np;
    // [DoNotSerialize] private PyModule scope;


    void Awake()
    {

    }
    void Start()
    {

        Runtime.PythonDLL = Application.streamingAssetsPath + "/embedded_python/python313.dll";
        PythonEngine.Initialize();



        using (Py.GIL())
        {
            pythonOutputRedirector = new PythonOutputRedirector();

            dynamic sys = Py.Import("sys");
            sys.stdout = PyObject.FromManagedObject(pythonOutputRedirector);
            sys.stderr = PyObject.FromManagedObject(pythonOutputRedirector);

        }


    }

    public bool TryExecuteCode(string input, out string result)
    {
        result = "";

        if (!PythonEngine.IsInitialized)
            return false;

        using (Py.GIL())
        {
            try
            {
                pythonOutputRedirector.Clear();
                // scope.Exec(input);
                using (PyModule tempScope = Py.CreateScope())
                {
                    tempScope.Exec(input);
                }

                string pyOut = pythonOutputRedirector.GetOutput();
                result = pyOut?.Trim() ?? "";
                return true;
            }
            catch (PythonException pyEx)
            {
                result = pyEx.ToString();
                return false;
            }
            catch (Exception e)
            {
                result = e.ToString();
                return false;
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

    void OnDestroy()
    {
        if (PythonEngine.IsInitialized)
        {
            try
            {
                // Clean shutdown on background thread
                PythonEngine.Shutdown();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Python shutdown error: {e}");
            }
        }
    }
}

