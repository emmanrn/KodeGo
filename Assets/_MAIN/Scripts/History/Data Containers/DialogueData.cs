using DIALOGUE;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class DialogueData
    {
        public string currentDialogue = "";
        public string currentSpeaker = "";

        public Color dialogueColor;
        public float dialogueScale;

        public Color speakerNameColor;
        public float speakerScale;

        public static DialogueData Capture()
        {
            DialogueData data = new DialogueData();

            var ds = DialogueSystem.instance;
            var dialogueTxt = ds.dialogueContainer.dialogueTxt;
            var nameTxt = ds.dialogueContainer.nameContainer.nameTxt;

            data.currentDialogue = dialogueTxt.text;
            data.dialogueColor = dialogueTxt.color;
            data.dialogueScale = dialogueTxt.fontSize;

            data.currentSpeaker = nameTxt.text;
            data.speakerNameColor = nameTxt.color;
            data.speakerScale = nameTxt.fontSize;

            return data;
        }

        public static void Apply(DialogueData data)
        {
            var ds = DialogueSystem.instance;
            var dialogueTxt = ds.dialogueContainer.dialogueTxt;
            var nameTxt = ds.dialogueContainer.nameContainer.nameTxt;

            // ds.conversationManager.archi
            dialogueTxt.text = data.currentDialogue;
            dialogueTxt.color = data.dialogueColor;
            dialogueTxt.fontSize = data.dialogueScale;


            nameTxt.text = data.currentSpeaker;
            if (nameTxt.text != string.Empty)
                ds.dialogueContainer.nameContainer.Show();
            else
                ds.dialogueContainer.nameContainer.Hide();
            nameTxt.color = data.speakerNameColor;
            nameTxt.fontSize = data.speakerScale;

        }
    }

}
