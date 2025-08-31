using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Pool;
using UnityEngine.U2D.IK;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        private const float DEFAULT_FADE_SPEED = 5f;
        public GameObject root;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueTxt;

        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        private Coroutine CO_Showing = null;
        private Coroutine CO_Hiding = null;
        public bool isShowing => CO_Showing != null;
        public bool isHiding => CO_Hiding != null;
        public bool isFading => isShowing || isHiding;
        public bool isVisible => CO_Showing != null || rootCG.alpha > 0;

        public void SetDialogueColor(Color color) => dialogueTxt.color = color;

        public Coroutine Show()
        {
            if (isShowing)
                return CO_Showing;
            else if (isHiding)
            {
                DialogueSystem.instance.StopCoroutine(CO_Hiding);
                CO_Hiding = null;
            }

            CO_Showing = DialogueSystem.instance.StartCoroutine(Fading(1));
            return CO_Showing;

        }

        public Coroutine Hide()
        {
            if (isHiding)
                return CO_Hiding;
            else if (isShowing)
            {
                DialogueSystem.instance.StopCoroutine(CO_Showing);
                CO_Showing = null;
            }

            CO_Hiding = DialogueSystem.instance.StartCoroutine(Fading(0));
            return CO_Hiding;


        }

        private IEnumerator Fading(float alpha)
        {
            CanvasGroup cg = rootCG;

            while (cg.alpha != alpha)
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED);
                yield return null;
            }
            CO_Showing = null;
            CO_Hiding = null;

        }
    }

}
