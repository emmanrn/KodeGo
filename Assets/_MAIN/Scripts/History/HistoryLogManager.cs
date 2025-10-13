using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HISTORY
{
    public class HistoryLogManager : MonoBehaviour
    {
        private const float LOG_STARTING_HEIGHT = 2f;
        private const float LOG_HEIGHT_PER_LINE = 2f;
        private const float LOG_DEFAULT_HEIGHT = 1f;
        private const float TEXT_DEFAULT_SCALE = 1f;

        private const string NAMETEXT_NAME = "NameTxt";
        private const string DIALOGUETEXT_NAME = "DialogueTxt";

        private float logScaling = 1f;

        [SerializeField] private Animator anim;
        [SerializeField] private GameObject logPrefab;

        HistoryManager manager => HistoryManager.instance;
        private List<HistoryLog> logs = new List<HistoryLog>();

        public bool isOpen { get; private set; } = false;
        [SerializeField] private Slider logScaleSlider;
        private float textScaling => logScaling * 3f;

        public void Open()
        {
            if (isOpen)
                return;

            anim.Play("Open");

            isOpen = true;
        }

        public void Close()
        {
            if (!isOpen)
                return;

            anim.Play("Close");

            isOpen = false;
        }

        public void AddLog(HistoryState state)
        {
            if (logs.Count >= HistoryManager.HISTORY_CACHE_LIMIT)
            {
                DestroyImmediate(logs[0].container);
                logs.RemoveAt(0);
            }

            CreateLog(state);
        }

        private void CreateLog(HistoryState state)
        {
            HistoryLog log = new HistoryLog();

            log.container = Instantiate(logPrefab, logPrefab.transform.parent);
            log.container.SetActive(true);

            log.nameTxt = log.container.transform.Find(NAMETEXT_NAME).GetComponent<TextMeshProUGUI>();
            log.dialogueTxt = log.container.transform.Find(DIALOGUETEXT_NAME).GetComponent<TextMeshProUGUI>();

            if (state.dialogue.currentSpeaker == string.Empty)
            {
                log.nameTxt.text = string.Empty;
            }
            else
            {
                log.nameTxt.text = state.dialogue.currentSpeaker;
                log.nameTxt.color = state.dialogue.speakerNameColor;
                log.nameFontSize = TEXT_DEFAULT_SCALE * state.dialogue.speakerScale;
                log.nameTxt.fontSize = log.nameFontSize + textScaling;
            }

            log.dialogueTxt.text = state.dialogue.currentDialogue;
            log.dialogueTxt.color = state.dialogue.dialogueColor;
            log.dialogueFontSize = TEXT_DEFAULT_SCALE * state.dialogue.dialogueScale;
            log.dialogueTxt.fontSize = log.dialogueFontSize + textScaling;

            FitLogToTxt(log);


            logs.Add(log);
        }

        private void FitLogToTxt(HistoryLog log)
        {
            RectTransform rect = log.dialogueTxt.GetComponent<RectTransform>();
            ContentSizeFitter textCSF = log.dialogueTxt.GetComponent<ContentSizeFitter>();

            textCSF.SetLayoutVertical();

            LayoutElement logLayout = log.container.GetComponent<LayoutElement>();
            float height = rect.rect.height;

            float percent = height / LOG_DEFAULT_HEIGHT;
            float extraScale = (LOG_HEIGHT_PER_LINE * percent) - LOG_HEIGHT_PER_LINE;
            float scale = LOG_STARTING_HEIGHT + extraScale;

            logLayout.preferredHeight = scale + textScaling;
            logLayout.preferredHeight += 2f * logScaling;
        }

        public void SetLogScaling()
        {
            logScaling = logScaleSlider.value;

            foreach (HistoryLog log in logs)
            {
                log.nameTxt.fontSize = log.nameFontSize + textScaling;
                log.dialogueTxt.fontSize = log.dialogueFontSize + textScaling;

                FitLogToTxt(log);
            }
        }

        // just ic case we want the log manager to clear the list if we want to
        public void Clear()
        {
            for (int i = 0; i < logs.Count; i++)
                DestroyImmediate(logs[i].container);

            logs.Clear();
        }

        public void Rebuild()
        {
            foreach (var state in manager.history)
                CreateLog(state);
        }
    }

}
