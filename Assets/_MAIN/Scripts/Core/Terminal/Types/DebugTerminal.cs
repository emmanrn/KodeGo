using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TERMINAL
{
    public class DebugTerminal : CodeTerminal<DebugTerminalConfig>
    {
        [Header("Debug Terminal")]
        [SerializeField] private GameObject codeBlockPrefab; // draggable block prefab
        [SerializeField] private GameObject slotPrefab;      // debug slot

        private List<DebugSlot> slots;
        private int attempts;
        private int prevHintIndex = -1;
        protected override void InitializeTerminal()
        {
            runBtn.onClick.RemoveAllListeners();
            slots = new List<DebugSlot>();
            runBtn.onClick.AddListener(Run);


            base.InitializeTerminal();

            expectedOutputTerminal.text = currentConfig.expectedOutput;

            attempts = 0;
            prevHintIndex = -1;
            outputTerminal.text = "";

            BuildInitialBlocks();
            if (currentPopup != null)
            {
                StopCoroutine(currentPopup);
                currentPopup = null;
            }
        }

        private void BuildInitialBlocks()
        {
            // Ensure slots are already built
            if (slots.Count != currentConfig.prefilledBlocks.Length)
            {
                Debug.LogError("Number of prefilled blocks does not match number of slots!");
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                var blockGO = ObjectPoolManager.SpawnObject(codeBlockPrefab, slots[i].transform, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                blockGO.GetComponentInChildren<TextMeshProUGUI>().text = currentConfig.prefilledBlocks[i];
                var codeBlock = blockGO.GetComponent<CodeBlock>();
                codeBlock.code = currentConfig.prefilledBlocks[i];

                // Set the parent of the code block to the slot initially
                blockGO.transform.SetParent(slots[i].transform);
                blockGO.transform.localPosition = Vector3.zero;
                // codeBlock.parentAfterDrag = slots[i].transform;
            }
        }

        public override void Run()
        {
            string code = GetFullCode();

            if (ContainsRecursion(code))
            {
                outputTerminal.color = Color.yellow;
                outputTerminal.text = "Error: recursion not allowed.";
                return;
            }

            bool success = interpreter.TryExecuteCode(code, out string output);
            outputTerminal.text = "";

            CheckOutput(output, currentConfig.expectedOutput);
            if (success)
            {
                CheckOutput(output, currentConfig.expectedOutput);
            }
            else
            {
                attempts++; // increment only once per run
                CheckHintThreshold();
                StartErrorPopup();
            }
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
                attempts++;
                bool thresholdReached = (attempts % MAX_WRONG_ATTEMPTS == 0) ? true : false;

                if (thresholdReached)
                    ShowHint();


                outputTerminal.color = Color.red;
                outputTerminal.text = output;

            }
        }

        public override void BuildLine(Transform lineParent, string line)
        {
            int searchIndex = 0;
            const float spaceWidth = 55f; // pixels per space
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
                    // Trim leading spaces if we already have an indent spacer
                    string trimmedBefore = before;
                    if (indentWidth > 0 && searchIndex == 0)
                        trimmedBefore = before.TrimStart(' ', '\t');

                    if (!string.IsNullOrEmpty(trimmedBefore))
                    {
                        var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                        // chunk.GetComponentInChildren<TextMeshProUGUI>().text = before;
                        chunk.GetComponentInChildren<TextMeshProUGUI>().text = trimmedBefore;
                    }
                }

                // Add slot (and indent spacer if first one)
                if (indentWidth > 0 && searchIndex == 0)
                {
                    var indentObj = ObjectPoolManager.SpawnObject(indentPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                    indentObj.transform.SetParent(lineParent, false);
                    indentObj.GetComponent<LayoutElement>().preferredWidth = indentWidth;
                }

                // Create drop target slot
                var slotGO = ObjectPoolManager.SpawnObject(slotPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                slotGO.transform.localScale = Vector3.one;
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
        private void CheckHintThreshold()
        {
            if (attempts % MAX_WRONG_ATTEMPTS == 0)
                ShowHint();
        }

        private void ShowHint()
        {
            if (currentConfig.hints == null || currentConfig.hints.Length == 0)
                return;

            int randomHintIndex;

            // If there’s only one hint, just show it.
            if (currentConfig.hints.Length == 1)
            {
                randomHintIndex = 0;
            }
            else
            {
                do
                {
                    randomHintIndex = Random.Range(0, currentConfig.hints.Length);
                }
                while (randomHintIndex == prevHintIndex);
            }

            prevHintIndex = randomHintIndex;
            PopupMenuManager.instance.ShowHintPopup(currentConfig.hints[randomHintIndex]);
        }
        protected override void OnClose()
        {
            AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResource(FilePaths.resources_sfx, "terminal_interact"));
            for (int i = codeContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = codeContainer.GetChild(i);
                ObjectPoolManager.ReleaseRecursive(child.gameObject);
            }
            slots.Clear();
        }
    }

}