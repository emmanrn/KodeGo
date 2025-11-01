using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TERMINAL
{
    public class PracticeTerminal : CodeTerminal<PracticeTermConfig>
    {

        [SerializeField] protected GameObject inputFieldPrefab; // Input field prefab (TMP_InputField)
        protected List<TMP_InputField> inputFields;
        private int attempts;
        private int prevHintIndex = -1;
        protected override void InitializeTerminal()
        {
            runBtn.onClick.RemoveAllListeners();
            inputFields = new List<TMP_InputField>();
            runBtn.onClick.AddListener(Run);

            base.InitializeTerminal();

            expectedOutputTerminal.text = currentConfig.expectedOutput;

            attempts = 0;
            prevHintIndex = -1;
            outputTerminal.text = "";

            if (currentPopup != null)
            {
                StopCoroutine(currentPopup);
                currentPopup = null;
            }

        }

        public override void BuildLine(Transform lineParent, string line)
        {
            // int searchIndex = 0;
            // while (true)
            // {
            //     int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
            //     if (nextIndex == -1)
            //     {
            //         string textChunk = line.Substring(searchIndex);
            //         if (!string.IsNullOrEmpty(textChunk))
            //         {
            //             var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
            //             chunk.GetComponentInChildren<TextMeshProUGUI>().text = textChunk;
            //         }
            //         break;
            //     }

            //     string beforeInput = line.Substring(searchIndex, nextIndex - searchIndex);
            //     if (!string.IsNullOrEmpty(beforeInput))
            //     {
            //         var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
            //         chunk.GetComponentInChildren<TextMeshProUGUI>().text = beforeInput;
            //     }

            //     // Input field
            //     var inputChunk = ObjectPoolManager.SpawnObject(inputFieldPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);

            //     inputChunk.transform.localScale = Vector3.one;
            //     var input = inputChunk.GetComponent<TMP_InputField>();
            //     input.text = "";
            //     inputFields.Add(input);

            //     searchIndex = nextIndex + INPUT_ID.Length;
            // }
            int searchIndex = 0;

            while (true)
            {
                int nextIndex = line.IndexOf(INPUT_ID, searchIndex);
                string beforeInput;

                if (nextIndex == -1)
                {
                    beforeInput = line.Substring(searchIndex);
                }
                else
                {
                    beforeInput = line.Substring(searchIndex, nextIndex - searchIndex);
                }

                // Always spawn a chunk, even if it's just whitespace
                var chunk = ObjectPoolManager.SpawnObject(codeChunkPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                chunk.transform.localScale = Vector3.one;

                // Replace tabs with spaces for TMP display
                chunk.GetComponentInChildren<TextMeshProUGUI>().text = beforeInput.Replace("\t", "    ");

                if (nextIndex == -1)
                    break;

                // Spawn input field
                var inputChunk = ObjectPoolManager.SpawnObject(inputFieldPrefab, lineParent, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                inputChunk.transform.localScale = Vector3.one;
                var input = inputChunk.GetComponent<TMP_InputField>();
                input.text = "";
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

            bool success = interpreter.TryExecuteCode(code, out string output);
            Debug.Log("Success " + success);

            outputTerminal.text = "";

            if (success)
            {
                CheckOutput(output, currentConfig.expectedOutput);
            }
            else
            {
                outputTerminal.text = output;
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
                output = string.IsNullOrEmpty(output) ? "None" : output;

                attempts++;

                CheckHintThreshold();

                outputTerminal.color = new Color(1, 0.33f, 0.33f);
                outputTerminal.text = output;

            }

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
            inputFields.Clear();

            if (currentPopup != null)
            {
                StopCoroutine(currentPopup);
                currentPopup = null;
            }

        }





    }

}