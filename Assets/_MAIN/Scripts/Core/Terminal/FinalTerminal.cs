using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTerminal : Terminal
{
    public override void Awake()
    {
        expectedOutputTerminal.text = outputCode;
        rootContainer.SetActive(false);
    }
    public override void CheckOutput(string output, string outputCode)
    {
        throw new System.NotImplementedException();
    }

    public override void Run()
    {
        string result = interpreter.RunCode(content);
        interpreter.ExecuteCode(result);
    }
}
