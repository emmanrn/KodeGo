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
    // [SerializeField] private GameObject content;
    // [SerializeField] private TextMeshProUGUI outputCode;
    [DoNotSerialize] private PyModule scope;

    // private GameStateManager gameState;

    void Awake()
    {

        // gameState = GameStateManager.instance;
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

    public string RunCode(GameObject content)
    {
        LineSlot[] slots = content.GetComponentsInChildren<LineSlot>();
        List<string> linesOfCodes = new List<string>();

        // now this is a convoluted mess just to deal with the fuvking tabs or indentation in the python code my god
        // so the reason why this looks the way it does is because that since python uses indentation for scope unity should be able to read the \t escape character
        // the problem however arised when trying to visulize the indent because the textmeshpro in the inspector doesnt show the \t (tab) so this happened as
        // a work around i guess. 
        // So basically how the terminal works is that you have slots, in those slots you have a bunch of texts which contains parts of the line, it could be 
        // hardcoded already or the player has to drag and drop the code block to it. The thing is that the one where the players drops the code is separate part
        // in the line, basically it has its own game object representing it. The structure goes liek this.
        //
        // Line Slot
        // > Line 
        // >> Line Part
        // >> Input Line Part - 1
        //
        // so they're separate.
        // since the \t cant be visualized through the game, we have to put a basiclly dummy line part that only contains the \t then add a margin to it, so that
        // we can visualize the indent.
        // after doing that we have this code. Which bsically gets all the line slots, looks through all its line parts
        // then we check if it has a \t on it, if it has more than one line part meaning one input and one hardcodded line part
        // we would then check if that has an indent, if it has exactly one indent then we combine the two strings WITH NO SPACES SEPARATING THEM.
        // otherwise if theres more than one indent, we first combine the indent with the next line part next to it, then combine the remaining parts into one string
        // THEN after that combine the combined part with tabs and the combined remaining into oone string, and separate i twith spaces AND THEN WE CAN ADD IT TO THE LIST
        // OF LINES THAT NEEDS TO BE EXECUTED.
        for (int i = 0; i < slots.Length; i++)
        {
            LineSlot slot = slots[i];
            if (slot.lines.Length > 1)
            {
                string line = slot.lines[0];
                string word = "";
                string wordWithTab = "";
                string[] lineWithTabs = new string[2];
                if (slot.GetNumberOfTabs() == 1)
                {
                    wordWithTab = line + slot.lines[1];
                    linesOfCodes.Add(wordWithTab);
                    continue;
                }
                else if (slot.GetNumberOfTabs() > 1)
                {
                    // thi i how we get if there are multiple tab
                    for (int k = 0; k < slot.GetNumberOfTabs(); k++)
                        wordWithTab += slot.lines[k];


                    wordWithTab += slot.lines[slot.GetNumberOfTabs()];
                    lineWithTabs[0] = wordWithTab;

                    for (int j = slot.GetNumberOfTabs() + 1; j < slot.lines.Length; j++)
                    {
                        word += slot.lines[j];
                    }
                    lineWithTabs[1] = string.Join(' ', lineWithTabs);
                    string res = string.Join(' ', lineWithTabs);
                    linesOfCodes.Add(res);
                    continue;

                }
                word = string.Join(' ', slot.lines);
                linesOfCodes.Add(word);


                continue;
            }

            linesOfCodes.Add(slot.lines[0]);
        }

        string result = string.Join("\n", linesOfCodes);
        return result;
    }

    public string ExecuteCode(string input)
    {
        // if (gameState.isPaused || gameState.isRunningDialogue)
        //     return;

        if (!PythonEngine.IsInitialized) return null;

        using (Py.GIL())
        {
            try
            {
                pythonOutputRedirector.Clear();
                scope.Exec(input);


                string pyOut = pythonOutputRedirector.GetOutput();


                if (!string.IsNullOrWhiteSpace(pyOut))
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

    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            PythonEngine.Shutdown();
        }
    }
}

