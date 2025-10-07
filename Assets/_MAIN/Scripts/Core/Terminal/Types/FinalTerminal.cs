using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace TERMINAL
{
    public class FinalTerminal : CodeTerminal<FinalTerminalConfig>
    {
        [Header("Sidebar & Prefabs")]
        [SerializeField] private Transform sidebarContainer;
        [SerializeField] private GameObject codeBlockPrefab;
        [SerializeField] private GameObject slotPrefab; // drop target

        private List<CodeDropTarget> slots = new();
        public override void Awake()
        {
            base.Awake();
            BuildSidebar();
            expectedOutputTerminal.text = config.expectedOutput;
            outputTerminal.text = "";
            rootContainer.SetActive(false);

            GameEvents.OnPlayerDied += PlayerDied;
        }

        private void BuildSidebar()
        {
            foreach (Transform child in sidebarContainer)
                Destroy(child.gameObject);

            foreach (string block in config.codeBlocks)
            {
                var go = Instantiate(codeBlockPrefab, sidebarContainer);
                go.GetComponentInChildren<TextMeshProUGUI>().text = block;
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
                    // Add remaining text
                    string remaining = line.Substring(searchIndex);
                    if (!string.IsNullOrEmpty(remaining))
                    {
                        var chunk = Instantiate(codeChunkPrefab, lineParent);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = remaining;
                    }
                    break;
                }

                // Add text before slot
                string before = line.Substring(searchIndex, nextIndex - searchIndex);
                if (!string.IsNullOrEmpty(before))
                {
                    var chunk = Instantiate(codeChunkPrefab, lineParent);
                    chunk.GetComponentInChildren<TextMeshProUGUI>().text = before;
                }

                // Add slot
                var slotGO = Instantiate(slotPrefab, lineParent);
                var slot = slotGO.GetComponent<CodeDropTarget>();
                slots.Add(slot);

                searchIndex = nextIndex + INPUT_ID.Length;
            }
        }

        public override string GetFullCode()
        {
            List<string> inputs = new List<string>();
            foreach (var slot in slots)
                inputs.Add(slot.currentCode ?? "");

            return BuildFullCode(inputs);
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

        private void PlayerDied()
        {
            CloseWindow();
            outputTerminal.text = "";
        }
    }

}