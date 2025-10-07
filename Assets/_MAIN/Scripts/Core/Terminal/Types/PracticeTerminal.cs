using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace TERMINAL
{
    public class PracticeTerminal : CodeTerminal<PracticeTermConfig>
    {

        [SerializeField] protected GameObject inputFieldPrefab; // Input field prefab (TMP_InputField)
        protected List<TMP_InputField> inputFields = new List<TMP_InputField>();
        public override void Awake()
        {
            base.Awake();
            expectedOutputTerminal.text = config.expectedOutput;
            outputTerminal.text = "";
            rootContainer.SetActive(false);
        }

        public override void BuildLine(Transform lineParent, string line)
        {
            // Early exit if empty line
            if (string.IsNullOrWhiteSpace(line))
            {
                Instantiate(codeChunkPrefab, lineParent).GetComponentInChildren<TextMeshProUGUI>().text = "";
                return;
            }

            // We'll track where the {input} tokens appear
            int searchIndex = 0;

            while (true)
            {
                int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
                if (nextIndex == -1)
                {
                    // Add remaining text (no more {input})
                    string textChunk = line.Substring(searchIndex);
                    if (!string.IsNullOrEmpty(textChunk))
                    {
                        var chunk = Instantiate(codeChunkPrefab, lineParent);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = textChunk;
                    }
                    break;
                }

                // Add text before the input
                string beforeInput = line.Substring(searchIndex, nextIndex - searchIndex);
                if (!string.IsNullOrEmpty(beforeInput))
                {
                    var chunk = Instantiate(codeChunkPrefab, lineParent);
                    chunk.GetComponentInChildren<TextMeshProUGUI>().text = beforeInput;
                }

                // Add input field
                var input = Instantiate(inputFieldPrefab, lineParent).GetComponent<TMP_InputField>();
                inputFields.Add(input);

                // Move past this token
                searchIndex = nextIndex + INPUT_ID.Length;
            }
        }


        public override string GetFullCode()
        {
            List<string> inputs = new List<string>();
            foreach (var field in inputFields)
                inputs.Add(field.text);

            return BuildFullCode(inputs);

        }




        public override void Run()
        {
            string code = GetFullCode();

            if (ContainsRecursion(code))
            {
                outputTerminal.color = Color.yellow;
                outputTerminal.text = "Error: Recursion is not allowed.";
                return;
            }

            string output = interpreter.ExecuteCode(code);

            outputTerminal.text = "";

            CheckOutput(output, config.expectedOutput);
        }

        public override void CheckOutput(string output, string outputCode)
        {
            output = output.Replace("\r\n", "\n").Trim();

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

}