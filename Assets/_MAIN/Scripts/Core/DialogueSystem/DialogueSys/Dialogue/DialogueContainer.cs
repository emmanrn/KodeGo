using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueTxt;

        private CanvasGroupController cgController;


        public void SetDialogueColor(Color color) => dialogueTxt.color = color;
        public void SetDialogueFontSize(float size) => dialogueTxt.fontSize = size;

        private bool initialized = false;
        public void Initialize()
        {
            if (initialized)
                return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);

    }

}
