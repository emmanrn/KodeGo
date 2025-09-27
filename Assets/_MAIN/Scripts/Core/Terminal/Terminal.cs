using System.Linq;
using TMPro;
using UnityEngine;

public abstract class Terminal : MonoBehaviour, IInteractable
{
    [TextArea(1, 20)]
    [SerializeField] protected string outputCode;
    [SerializeField] protected GameObject rootContainer;
    [SerializeField] protected GameObject content;
    [SerializeField] protected TextMeshProUGUI outputTerminal;
    [SerializeField] protected TextMeshProUGUI expectedOutputTerminal;
    [SerializeField] protected InputReader inputReader;
    [SerializeField] protected Interpreter interpreter;
    public bool isInteractable() => true;

    public virtual void Awake() { }
    public void Interact()
    {
        inputReader.SetUI();
        rootContainer.SetActive(true);
    }
    public void CloseWindow()
    {
        inputReader.SetPlayerMovement();
        rootContainer.SetActive(false);
    }
    public abstract void Run();
    public abstract void CheckOutput(string output, string outputCode);

    protected bool ContainsRecursion(string code)
    {
        var lines = code.Split('\n');

        string currentFunc = null;
        int funcIndentLevel = 0;

        foreach (var rawLine in lines)
        {
            string line = rawLine.Replace("\r", "");
            if (string.IsNullOrWhiteSpace(line)) continue;

            int indent = line.TakeWhile(c => c == '\t').Count();

            // if we detect if we are in the initialiation of a function
            if (line.TrimStart().StartsWith("def "))
            {
                // Extract the function name
                currentFunc = line.Trim().Split(' ')[1].Split('(')[0];
                funcIndentLevel = indent;
                continue;
            }

            // If we're inside a function body
            if (currentFunc != null && indent > funcIndentLevel)
            {
                if (line.Contains(currentFunc + "("))
                {
                    Debug.LogWarning($"Recursion detected in function: {currentFunc}");
                    return true;
                }
            }

            // If indentation drops back, we left the function
            if (currentFunc != null && indent <= funcIndentLevel)
            {
                currentFunc = null;
                funcIndentLevel = 0;
            }
        }

        return false;
    }

}
