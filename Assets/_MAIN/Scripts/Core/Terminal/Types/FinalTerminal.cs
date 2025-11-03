using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MAIN_GAME;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TERMINAL
{
    public class FinalTerminal : CodeTerminal<FinalTerminalConfig>
    {
        [Header("Sidebar & Prefabs")]
        [SerializeField] private Transform sidebarContainer;
        [SerializeField] private GameObject codeBlockPrefab;
        [SerializeField] private GameObject slotPrefab; // drop target
        [SerializeField] private int totalBlocks = 3;
        [SerializeField] protected GameObject blockCounter;

        private List<CodeDropTarget> slots;

        protected override void InitializeTerminal()
        {
            runBtn.onClick.RemoveAllListeners();
            slots = new List<CodeDropTarget>();
            runBtn.onClick.AddListener(Run);

            base.InitializeTerminal();

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
            const float spaceWidth = 55f;
            const int tabSize = 4;

            bool hasInput = line.Contains(INPUT_ID);

            // Determine if the line starts with indentation (space or tab)
            bool startsWithIndent = line.Length > 0 && (line[0] == ' ' || line[0] == '\t');

            float indentWidth = 0f;
            if (hasInput && startsWithIndent)
            {
                int indentCount = 0;
                foreach (char c in line)
                {
                    if (c == ' ') indentCount++;
                    else if (c == '\t') indentCount += tabSize;
                    else break;
                }

                if (indentCount > 0)
                    indentWidth = indentCount * spaceWidth;
            }

            // ✅ Spawn the indent spacer first (if needed)
            if (indentWidth > 0)
            {
                var indentObj = ObjectPoolManager.SpawnObject(indentPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                indentObj.transform.SetParent(lineParent, false);
                indentObj.GetComponent<LayoutElement>().preferredWidth = indentWidth;
            }

            // Now build chunks
            while (true)
            {
                int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
                if (nextIndex == -1)
                {
                    // Add remaining text (no inputs left)
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
                    string trimmedBefore = before;
                    if (indentWidth > 0 && searchIndex == 0)
                        trimmedBefore = before.TrimStart(' ', '\t');

                    if (!string.IsNullOrEmpty(trimmedBefore))
                    {
                        var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = trimmedBefore;
                    }
                }

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
                StartCoroutine(OnPlayerWin());
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

            bool success = interpreter.TryExecuteCode(code, out string output);
            outputTerminal.text = "";

            if (success)
            {
                CheckOutput(output, currentConfig.expectedOutput);
            }
            else
            {
                StartErrorPopup();
            }

        }


        private IEnumerator OnPlayerWin()
        {
            blockCounter.SetActive(false);
            LevelProgressManager.SetPlayerLevelWin(levelName);
            LevelProgressManager.UnlockTitle(levelName, GameManager.instance.titleToBeUnlocked);
            GameSave.activeFile.Save();

            yield return StartCoroutine(ShowVictory());
            Transition.instance.LoadLevel(levelName, GameManager.instance.fileToRead);
        }

        private void PlayerDied()
        {
            ClickCloseWindow();
            outputTerminal.text = "";
        }

        private IEnumerator ShowVictory()
        {
            PopupMenuManager.instance.ShowVictoryPopup("PASSED");
            yield return new WaitForSeconds(1.25f);
            PopupMenuManager.instance.HideVictoryPopup();
            ClickCloseWindow();
            yield return new WaitForSeconds(1f);
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