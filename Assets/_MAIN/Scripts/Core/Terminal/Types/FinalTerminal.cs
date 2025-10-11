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

        private List<CodeDropTarget> slots;
        protected override void InitializeTerminal()
        {
            slots = new List<CodeDropTarget>();
            runBtn.onClick.AddListener(Run);
            base.InitializeTerminal();
            rootContainer.SetActive(true);
            BuildSidebar();
            expectedOutputTerminal.text = config.expectedOutput;
            outputTerminal.text = "";

            GameEvents.OnPlayerDied += PlayerDied;
        }

        private void BuildSidebar()
        {
            foreach (Transform child in sidebarContainer)
                Destroy(child.gameObject);

            foreach (string block in config.codeBlocks)
            {
                var go = ObjectPoolManager.SpawnObject(codeBlockPrefab, sidebarContainer, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);

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
                        var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = remaining;
                    }
                    break;
                }

                // Add text before slot
                string before = line.Substring(searchIndex, nextIndex - searchIndex);
                if (!string.IsNullOrEmpty(before))
                {
                    var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                    chunk.GetComponentInChildren<TextMeshProUGUI>().text = before;
                }

                // Add slot
                var slotGO = ObjectPoolManager.SpawnObject(slotPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
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

        protected override void OnClose()
        {
            for (int i = codeContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = codeContainer.GetChild(i);
                ObjectPoolManager.ReleaseRecursive(child.gameObject);
            }
            for (int i = sidebarContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = sidebarContainer.GetChild(i);
                ObjectPoolManager.ReleaseRecursive(child.gameObject);
            }
            slots.Clear();
        }
    }

}