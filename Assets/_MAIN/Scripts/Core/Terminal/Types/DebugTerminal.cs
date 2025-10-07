using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TERMINAL
{
    public class DebugTerminal : CodeTerminal<DebugTerminalConfig>
    {
        [Header("Debug Terminal")]
        [SerializeField] private GameObject codeBlockPrefab; // draggable block prefab
        [SerializeField] private GameObject slotPrefab;      // drop target prefab

        private List<DebugSlot> slots = new List<DebugSlot>();
        public override void Awake()
        {
            base.Awake();
            expectedOutputTerminal.text = config.expectedOutput;
            outputTerminal.text = "";
            rootContainer.SetActive(false);

            BuildInitialBlocks();
        }

        private void BuildInitialBlocks()
        {
            // Ensure slots are already built
            if (slots.Count != config.prefilledBlocks.Length)
            {
                Debug.LogError("Number of prefilled blocks does not match number of slots!");
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                var blockGO = Instantiate(codeBlockPrefab, slots[i].transform);
                blockGO.GetComponentInChildren<TextMeshProUGUI>().text = config.prefilledBlocks[i];
                var codeBlock = blockGO.GetComponent<CodeBlock>();
                codeBlock.code = config.prefilledBlocks[i];

                // Set the parent of the code block to the slot initially
                blockGO.transform.SetParent(slots[i].transform);
                blockGO.transform.localPosition = Vector3.zero;
                // codeBlock.parentAfterDrag = slots[i].transform;
            }
        }

        public override void Run()
        {
            string result = GetFullCode();
            Debug.Log(result);

            if (ContainsRecursion(result))
            {
                outputTerminal.color = Color.yellow;
                outputTerminal.text = "Error: recursion not allowed.";
                return;
            }

            string output = interpreter.ExecuteCode(result);
            outputTerminal.text = "";

            CheckOutput(output, config.expectedOutput);
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

        public override void BuildLine(Transform lineParent, string line)
        {
            int searchIndex = 0;

            while (true)
            {
                int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
                if (nextIndex == -1)
                {
                    string remaining = line.Substring(searchIndex);
                    if (!string.IsNullOrEmpty(remaining))
                    {
                        var chunk = Instantiate(codeChunkPrefab, lineParent);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = remaining;
                    }
                    break;
                }

                // Text before input
                string before = line.Substring(searchIndex, nextIndex - searchIndex);
                if (!string.IsNullOrEmpty(before))
                {
                    var chunk = Instantiate(codeChunkPrefab, lineParent);
                    chunk.GetComponentInChildren<TextMeshProUGUI>().text = before;
                }

                // Create drop target slot
                var slotGO = Instantiate(slotPrefab, lineParent);
                var slot = slotGO.GetComponent<DebugSlot>();
                slots.Add(slot);

                searchIndex = nextIndex + INPUT_ID.Length;
            }
        }

        public override string GetFullCode()
        {
            List<string> inputs = new List<string>();
            foreach (var slot in slots)
            {
                CodeBlock codeBlock = slot.GetComponentInChildren<CodeBlock>();
                inputs.Add(codeBlock.code ?? "");

            }

            return BuildFullCode(inputs);
        }
    }

}