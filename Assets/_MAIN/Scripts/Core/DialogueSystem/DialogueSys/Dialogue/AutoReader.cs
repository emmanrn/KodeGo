using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        private Color TOGGLE_OFF_COLOR = new Color(0.28f, 0.52f, 0.4f);
        private Color TOGGLE_OFF_COLOR_TEXT = new Color(1f, 1f, 1f);
        private Color TOGGLE_ON_COLOR = new Color(0.51f, 0.96f, 0.74f);
        private Color TOGGLE_ON_COLOR_TEXT = new Color(0f, 0f, 0f);

        private ConversationManager conversationManager;
        private TextArchitect archi => conversationManager.archi;

        public bool skip { get; set; } = false;
        public float speed { get; set; } = 1f;
        public bool isOn => CO_Running != null;
        private Coroutine CO_Running = null;

        // [SerializeField] private TextMeshProUGUI statusTxt;
        [SerializeField] private Image autoImg;
        [SerializeField] private Image skipImg;
        [SerializeField] private Text autoText;
        [SerializeField] private Text skipText;
        [HideInInspector] public bool allowToggle = true;


        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            skipImg.color = TOGGLE_OFF_COLOR;
            skipText.color = TOGGLE_OFF_COLOR_TEXT;
            autoImg.color = TOGGLE_OFF_COLOR;
            autoText.color = TOGGLE_OFF_COLOR_TEXT;
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
            autoImg.color = TOGGLE_OFF_COLOR;
            autoText.color = TOGGLE_OFF_COLOR_TEXT;
            skipImg.color = TOGGLE_OFF_COLOR;
            skipText.color = TOGGLE_OFF_COLOR_TEXT;
        }

        private IEnumerator AutoRead()
        {
            yield return new WaitForEndOfFrame();
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!archi.isBuilding && archi.currentText != string.Empty)
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

            if (skip)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
            {
                skipImg.color = TOGGLE_OFF_COLOR;
                skipText.color = TOGGLE_OFF_COLOR_TEXT;
                autoImg.color = TOGGLE_ON_COLOR;
                autoText.color = TOGGLE_ON_COLOR_TEXT;
            }
            else
            {
                autoImg.color = TOGGLE_OFF_COLOR;
                autoText.color = TOGGLE_OFF_COLOR_TEXT;
            }
            skip = false;
        }
        public void ToggleSkip()
        {
            if (!allowToggle)
                return;

            if (!skip)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
            {
                autoImg.color = TOGGLE_OFF_COLOR;
                autoText.color = TOGGLE_OFF_COLOR_TEXT;
                skipImg.color = TOGGLE_ON_COLOR;
                skipText.color = TOGGLE_ON_COLOR_TEXT;
            }
            else
            {
                skipImg.color = TOGGLE_OFF_COLOR;
                skipText.color = TOGGLE_OFF_COLOR_TEXT;
            }
            skip = true;
        }
    }
}
