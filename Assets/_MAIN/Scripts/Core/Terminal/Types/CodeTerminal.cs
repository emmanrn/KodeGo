using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace TERMINAL
{
    public abstract class CodeTerminal<T> : Terminal
        where T : CodeTerminalConfig
    {

        protected const string INPUT_ID = "{input}";

        [Header("Code Terminal Config")]
        [SerializeField] protected T[] configs;
        protected T currentConfig;
        [SerializeField] protected Transform codeContainer; // Vertical layout group
        [SerializeField] protected GameObject codeLinePrefab; // Horizontal line prefab
        [SerializeField] protected GameObject codeChunkPrefab; // Code text prefab
        [SerializeField] protected Button closeBtn;
        [SerializeField] protected Button runBtn;

        protected const int MAX_WRONG_ATTEMPTS = 5;
        void Start()
        {
            currentConfig = QuestionManager.instance.GetRandomQuestion(terminalType) as T;

        }

        protected override void InitializeTerminal()
        {
            // pool = PoolManager.instance;
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(ClickCloseWindow);
            BuildCodeUI();
            rootContainer.SetActive(true);
            anim.Play("Open");
        }



        public abstract void BuildLine(Transform lineParent, string line);
        public abstract string GetFullCode();

        public virtual void BuildCodeUI()
        {
            if (currentConfig == null)
            {
                Debug.LogError($"[{name}] Missing CodeTerminalConfig!");
                return;
            }

            // clear previous UI
            foreach (Transform child in codeContainer)
            {
                ReturnObjectToPool(child.gameObject);
            }

            // build code lines from config
            foreach (var line in currentConfig.codeLines)
            {
                GameObject lineGO = ObjectPoolManager.SpawnObject(codeLinePrefab, codeContainer, Quaternion.identity, ObjectPoolManager.PoolType.GameObjects);
                lineGO.transform.localScale = Vector3.one;
                BuildLine(lineGO.transform, line);
            }
        }

        protected string BuildFullCode(List<string> userInputs)
        {
            var codeBuilder = new System.Text.StringBuilder();
            int inputIndex = 0;

            foreach (string line in currentConfig.codeLines)
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

        protected void ReturnObjectToPool(GameObject container)
        {
            ObjectPoolManager.ReleaseRecursive(container);
        }


    }
}

