using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MAIN_GAME;
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
        [SerializeField] private int totalBlocks = 3;

        private List<CodeDropTarget> slots;
        protected override void InitializeTerminal()
        {
            runBtn.onClick.RemoveListener(Run);
            slots = new List<CodeDropTarget>();
            runBtn.onClick.AddListener(Run);

            base.InitializeTerminal();
            rootContainer.SetActive(true);

            BuildSidebar();

            expectedOutputTerminal.text = currentConfig.expectedOutput;
            outputTerminal.text = "";

        }

        private void OnEnable()
        {
            GameEvents.OnPlayerDied += PlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerDied -= PlayerDied;
        }

        private void BuildSidebar()
        {
            int collectedCodeBlocks = CountCollectedBlocks();

            for (int i = 0; i < currentConfig.codeBlocks.Length; i++)
            // foreach (string block in config.codeBlocks)
            {
                var block = currentConfig.codeBlocks[i];
                var go = ObjectPoolManager.SpawnObject(codeBlockPrefab, sidebarContainer, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);

                go.GetComponentInChildren<TextMeshProUGUI>().text = block;
                go.GetComponent<CodeBlock>().code = block;

                bool unlocked = i < collectedCodeBlocks;
                go.SetActive(unlocked);
            }
        }

        private int CountCollectedBlocks()
        {
            return LevelProgressManager.runtime[GameManager.instance.LEVEL_NAME].collectedBlocks;
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

        public override void CheckOutput(string output, string outputCode)
        {
            output = output.Replace("\r\n", "\n").Trim();

            if (output == outputCode)
            {
                Debug.Log("Correct");
                outputTerminal.color = Color.green;
                outputTerminal.text = output;
                OnPlayerWin();
            }
            else
            {
                outputTerminal.color = Color.red;
                outputTerminal.text = output;


                GeneralManager.instance.Player?.TakeDamage(1);
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

            CheckOutput(output, currentConfig.expectedOutput);

        }

        private void OnPlayerWin()
        {
            LevelProgressManager.SetPlayerLevelWin(levelName);
            GameSave.activeFile.Save();
            Transition.instance.LoadLevel(levelName);
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