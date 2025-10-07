using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTerminalConfig : ScriptableObject
{
    [Header("Code Lines")]
    [Tooltip("Each line of code to show in the terminal. Use {input} for blanks.")]
    [TextArea(1, 10)]
    public string[] codeLines;

    [Header("Expected Output")]
    [Tooltip("The correct output when the user's code runs successfully.")]
    [TextArea(1, 10)]
    public string expectedOutput;
}
