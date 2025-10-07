using System.Collections.Generic;
using UnityEngine;

namespace TERMINAL
{
    public abstract class CodeTerminal<T> : Terminal
        where T : CodeTerminalConfig
    {

        protected const string INPUT_ID = "{input}";

        [Header("Code Terminal Config")]
        [SerializeField] protected T config;
        [SerializeField] protected Transform codeContainer; // Vertical layout group
        [SerializeField] protected GameObject codeLinePrefab; // Horizontal line prefab
        [SerializeField] protected GameObject codeChunkPrefab; // Code text prefab

        public override void Awake()
        {
            base.Awake();
            BuildCodeUI();
        }



        public abstract void BuildLine(Transform lineParent, string line);
        public abstract string GetFullCode();

        public virtual void BuildCodeUI()
        {
            if (config == null)
            {
                Debug.LogError($"[{name}] Missing CodeTerminalConfig!");
                return;
            }

            // clear previous UI
            foreach (Transform child in codeContainer)
                Destroy(child.gameObject);

            // build code lines from config
            foreach (var line in config.codeLines)
            {
                GameObject lineGO = Instantiate(codeLinePrefab, codeContainer);
                BuildLine(lineGO.transform, line);
            }
        }

        protected string BuildFullCode(List<string> userInputs)
        {
            var codeBuilder = new System.Text.StringBuilder();
            int inputIndex = 0;

            foreach (string line in config.codeLines)
            {
                int startIndex = 0;
                int placeholderIndex;

                while ((placeholderIndex = line.IndexOf("{input}", startIndex)) != -1)
                {
                    // Add text before the {input} placeholder
                    codeBuilder.Append(line.Substring(startIndex, placeholderIndex - startIndex));

                    // Add user input if available
                    if (inputIndex < userInputs.Count)
                        codeBuilder.Append(userInputs[inputIndex]);

                    inputIndex++;
                    startIndex = placeholderIndex + "{input}".Length;
                }

                // Add remaining text after the last placeholder
                codeBuilder.AppendLine(line.Substring(startIndex));
            }

            return codeBuilder.ToString();
        }



        // public override void CheckOutput(string output, string outputCode)
        // {
        //     output = output.Replace("\r\n", "\n").Trim();
        //     Debug.Log(outputCode);

        //     if (output == outputCode)
        //     {
        //         Debug.Log("Correct");
        //         outputTerminal.color = Color.green;
        //         outputTerminal.text = output;
        //     }
        //     else
        //     {
        //         outputTerminal.color = Color.red;
        //         outputTerminal.text = output;

        //     }
        // }

        // public override void Run()
        // {
        //     string result = interpreter.RunCode(content);

        //     if (ContainsRecursion(result))
        //     {
        //         outputTerminal.color = Color.yellow;
        //         outputTerminal.text = "Error: Recursion is not allowed.";
        //         return;
        //     }

        //     string output = interpreter.ExecuteCode(result);

        //     outputTerminal.text = "";

        //     CheckOutput(output, outputCode);
        // }

    }

}