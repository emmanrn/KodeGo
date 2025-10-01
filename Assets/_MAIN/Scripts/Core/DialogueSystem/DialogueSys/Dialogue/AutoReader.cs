using System.Collections;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    public class AutoReader : MonoBehaviour
    {
        private const int DEFAULT_CHARACTERS_READ_PER_SECOND = 18;
        private const float READ_TIME_PADDING = 0.5f;
        private const float MAX_READ_TIME = 99f;
        private const float MIN_READ_TIME = 1f;
        private const string STATUS_TXT_AUTO = "Auto";
        private const string STATUS_TXT_SKIP = "Skipping";

        private ConversationManager conversationManager;
        private TextArchitect archi => conversationManager.archi;

        public bool skip { get; set; } = false;
        public float speed { get; set; } = 1f;
        public bool isOn => CO_Running != null;
        private Coroutine CO_Running = null;

        [SerializeField] private TextMeshProUGUI statusTxt;
        [HideInInspector] public bool allowToggle = true;


        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            statusTxt.text = string.Empty;
        }

        public void Enable()
        {
            if (isOn)
                return;
            CO_Running = StartCoroutine(AutoRead());
        }

        public void Disable()
        {
            if (!isOn)
                return;

            StopCoroutine(CO_Running);
            skip = false;
            CO_Running = null;
            statusTxt.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!archi.isBuilding && archi.currentTxt != string.Empty)
                DialogueSystem.instance.OnSystemPromptNext();

            while (conversationManager.isRunning)
            {
                // read and wait
                if (!skip)
                {
                    while (!archi.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeStarted = Time.time;

                    while (archi.isBuilding || conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeToRead = Mathf.Clamp(((float)archi.tmpro.textInfo.characterCount / DEFAULT_CHARACTERS_READ_PER_SECOND), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = (timeToRead / speed) + READ_TIME_PADDING;

                    yield return new WaitForSeconds(timeToRead);
                }
                // skip
                if (skip)
                {
                    archi.ForceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialogueSystem.instance.OnSystemPromptNext();
            }

            Disable();
        }

        public void ToggleAuto()
        {
            if (!allowToggle)
                return;

            skip = false;
            if (skip)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                {
                    Disable();
                }
            }

            if (isOn)
                statusTxt.text = STATUS_TXT_AUTO;
        }
        public void ToggleSkip()
        {
            if (!allowToggle)
                return;

            skip = true;
            if (!skip)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                {
                    Disable();
                }
            }

            if (isOn)
                statusTxt.text = STATUS_TXT_SKIP;
        }
    }
}
