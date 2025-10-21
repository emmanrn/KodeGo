using System.Collections;
using System.Linq;
using MAIN_GAME;
using TMPro;
using UnityEngine;

namespace TERMINAL
{
    public abstract class Terminal : MonoBehaviour, IInteractable
    {
        [SerializeField] protected GameObject rootContainer;
        [SerializeField] protected TextMeshProUGUI outputTerminal;
        [SerializeField] protected TextMeshProUGUI expectedOutputTerminal;
        [SerializeField] protected InputReader inputReader;
        [SerializeField] protected Interpreter interpreter;
        [SerializeField] protected Animator anim;
        private bool interactable = true;
        protected string levelName => GameManager.instance.LEVEL_NAME;
        public bool isInteractable() => interactable;

        void Awake()
        {
            rootContainer.SetActive(false);
        }

        public void Interact()
        {
            if (isInteractable())
            {
                inputReader.SetUI();
                InitializeTerminal();
                rootContainer.SetActive(true);
            }
        }

        protected abstract void InitializeTerminal();

        public void ClickCloseWindow()
        {
            StartCoroutine(CloseWindow());
        }

        private IEnumerator CloseWindow()
        {
            anim.Play("Close");
            yield return new WaitForSeconds(0.2f);

            inputReader.SetPlayerMovement();
            rootContainer.SetActive(false);

            OnClose();
        }

        public abstract void Run();
        public abstract void CheckOutput(string output, string outputCode);

        protected bool ContainsRecursion(string code)
        {
            var lines = code.Split('\n');

            string currentFunc = null;
            int funcIndentLevel = -1;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Replace("\r", "");
                if (string.IsNullOrWhiteSpace(line)) continue;

                int indent = line.TakeWhile(c => c == '\t').Count();

                // if we detect if we are in the initialiation of a function
                if (line.TrimStart().StartsWith("def "))
                {
                    // Extract the function name
                    currentFunc = line.Trim().Split(' ')[0].Split('(')[0];
                    funcIndentLevel = indent;
                    continue;
                }

                // If we're inside a function body
                if (currentFunc != null && indent > funcIndentLevel)
                {
                    if (line.Contains(currentFunc + "("))
                    {
                        Debug.LogWarning($"Recursion detected in function: {currentFunc}");
                        return true;
                    }
                }

                // If indentation drops back, we left the function
                if (currentFunc != null && indent <= funcIndentLevel)
                {
                    currentFunc = null;
                    funcIndentLevel = -1;
                }
            }

            return false;
        }

        protected virtual void OnClose() { }
    }

}