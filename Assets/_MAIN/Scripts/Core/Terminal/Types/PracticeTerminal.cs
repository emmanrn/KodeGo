using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TERMINAL
{
    public class PracticeTerminal : CodeTerminal<PracticeTermConfig>
    {

        [SerializeField] protected GameObject inputFieldPrefab; // Input field prefab (TMP_InputField)
        protected List<TMP_InputField> inputFields;
        protected override void InitializeTerminal()
        {
            inputFields = new List<TMP_InputField>();
            runBtn.onClick.AddListener(Run);
            base.InitializeTerminal();
            expectedOutputTerminal.text = config.expectedOutput;
            outputTerminal.text = "";
        }

        public override void BuildLine(Transform lineParent, string line)
        {
            int searchIndex = 0;
            while (true)
            {
                int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
                if (nextIndex == -1)
                {
                    string textChunk = line.Substring(searchIndex);
                    if (!string.IsNullOrEmpty(textChunk))
                    {
                        var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = textChunk;
                    }
                    break;
                }

                string beforeInput = line.Substring(searchIndex, nextIndex - searchIndex);
                if (!string.IsNullOrEmpty(beforeInput))
                {
                    var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                    chunk.GetComponentInChildren<TextMeshProUGUI>().text = beforeInput;
                }

                // Input field
                var inputChunk = ObjectPoolManager.SpawnObject(inputFieldPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);

                inputChunk.transform.localScale = Vector3.one;
                var input = inputChunk.GetComponent<TMP_InputField>();
                inputFields.Add(input);

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

        protected override void OnClose()
        {
            for (int i = codeContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = codeContainer.GetChild(i);
                ObjectPoolManager.ReleaseRecursive(child.gameObject);
            }
            inputFields.Clear();
        }





    }

}