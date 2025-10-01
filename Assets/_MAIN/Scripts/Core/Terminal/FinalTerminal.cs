using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTerminal : Terminal
{
    public override void Awake()
    {
        expectedOutputTerminal.text = outputCode;
        outputTerminal.text = "";
        rootContainer.SetActive(false);

        GameEvents.OnPlayerDied += PlayerDied;
    }


    private void OnDestroy()
    {
        GameEvents.OnPlayerDied -= PlayerDied;
    }
    public override void CheckOutput(string output, string outputCode)
    {
        output = output.Replace("\r\n", "\n").Trim();
        Debug.Log(outputCode);

        if (output == outputCode)
        {
            Debug.Log("Correct");
            outputTerminal.color = Color.green;
            outputTerminal.text = output;
        }
        else
        {
            outputTerminal.color = Color.red;
            outputTerminal.text = output;


            GameManager.instance.Player?.TakeDamage(1);
        }
    }

    public override void Run()
    {
        string result = interpreter.RunCode(content);
        interpreter.ExecuteCode(result);
    }

    private void PlayerDied()
    {
        CloseWindow();
        outputTerminal.text = "";
    }
}
