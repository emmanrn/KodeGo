using UnityEngine;

public class PracticeTerminal : Terminal
{
    public override void Awake()
    {
        expectedOutputTerminal.text = outputCode;
        outputTerminal.text = "";
        rootContainer.SetActive(false);

    }

    public override void Run()
    {
        string result = interpreter.RunCode(content);

        if (ContainsRecursion(result))
        {
            outputTerminal.color = Color.yellow;
            outputTerminal.text = "Error: Recursion is not allowed.";
            return;
        }

        string output = interpreter.ExecuteCode(result);

        outputTerminal.text = "";

        CheckOutput(output, outputCode);
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

        }

    }





}
