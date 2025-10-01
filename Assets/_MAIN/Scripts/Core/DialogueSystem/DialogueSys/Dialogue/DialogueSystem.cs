using System.Collections.Generic;
using System.Security.Cryptography;
using CHARACTERS;
using UnityEngine;


namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        // TODO
        // add a feature that also changes the player icons as well (fuck thts gonna be hard i think)
        [SerializeField] private DialogueSysConfig_SO _config;
        public DialogueSysConfig_SO config => _config;
        public DialogueContainer dialogueContainer = new DialogueContainer();
        public ConversationManager conversationManager { get; private set; }
        private TextArchitect archi;
        private AutoReader autoReader;
        public static DialogueSystem instance { get; private set; }
        public delegate void DialogueSysEvent();
        public event DialogueSysEvent onUserPromptNext;
        public event DialogueSysEvent onClear;
        public bool isRunningConversation => conversationManager.isRunning;
        public DialogueContinuePrompt prompt;

        void Awake()
        {
            if (instance == null)
            {

                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);

        }

        private void OnDisable()
        {

        }

        bool initialized = false;
        private void Initialize()
        {
            if (initialized)
                return;

            archi = new TextArchitect(dialogueContainer.dialogueTxt);
            conversationManager = new ConversationManager(archi);

            if (TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager);
        }

        public void OnUserPromptNext()
        {
            Debug.Log("User promt next");
            onUserPromptNext?.Invoke();

            // basically if the player clicks again while still auto reading
            // then the player will take control again after that
            if (autoReader != null && autoReader.isOn)
                autoReader.Disable();
        }
        public void OnSystemPromptNext()
        {
            onUserPromptNext?.Invoke();
        }

        public void OnSystemPromptClear()
        {
            onClear?.Invoke();
        }

        public void OnStartViewingHistory()
        {
            prompt.Hide();
            autoReader.allowToggle = false;
            conversationManager.allowUserPrompts = false;

            if (autoReader.isOn)
                autoReader.Disable();
        }

        public void OnStopViewingHistory()
        {
            prompt.Show();
            autoReader.allowToggle = true;
            conversationManager.allowUserPrompts = true;
        }


        public void ApplySpeakerData(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerData(config);
        }

        public void ApplySpeakerData(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            float fontSize = this.config.defaultDialogueFontSize * this.config.dialogueFontScale * config.dialogueFontScale;
            dialogueContainer.SetDialogueFontSize(fontSize);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            fontSize = this.config.defaultNameFontSize * config.nameFontScale;
            dialogueContainer.nameContainer.SetNameFontSize(fontSize);

        }
        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameTxt.text = "";

            }
        }
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        // this is basically where what lines/dialogues we want to show on the screen/dialogue panel
        public Coroutine Say(string speaker, string dialogue)
        {
            Debug.Log(speaker);
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> lines)
        {
            Conversation conversation = new Conversation(lines);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public void StopDialogue() => conversationManager.StopConversation();
    }

}

