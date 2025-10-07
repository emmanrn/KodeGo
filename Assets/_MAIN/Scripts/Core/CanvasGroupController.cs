using System.Collections;
using UnityEngine;
using DIALOGUE;
using UnityEngine.UIElements;

// this script is for controlling the visibility of canvas groups
public class CanvasGroupController
{
    private const float DEFAULT_FADE_SPEED = 5f;

    private MonoBehaviour owner;
    private CanvasGroup rootCG;
    private Coroutine CO_Showing = null;
    private Coroutine CO_Hiding = null;
    public bool isShowing => CO_Showing != null;
    public bool isHiding => CO_Hiding != null;
    public bool isFading => isShowing || isHiding;
    public bool isVisible => CO_Showing != null || rootCG.alpha > 0;
    public float alpha { get { return rootCG.alpha; } set { rootCG.alpha = value; } }

    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCG)
    {
        this.owner = owner;
        this.rootCG = rootCG;
    }

    public Coroutine Show(float speed = 1f, bool immediate = false)
    {
        if (isShowing)
            return CO_Showing;
        else if (isHiding)
        {
            DialogueSystem.instance.StopCoroutine(CO_Hiding);
            CO_Hiding = null;
        }

        CO_Showing = DialogueSystem.instance.StartCoroutine(Fading(1, speed, immediate));
        rootCG.gameObject.SetActive(true);
        return CO_Showing;

    }

    public Coroutine Hide(float speed = 1f, bool immediate = false)
    {
        if (isHiding)
            return CO_Hiding;
        else if (isShowing)
        {
            DialogueSystem.instance.StopCoroutine(CO_Showing);
            CO_Showing = null;
        }

        CO_Hiding = DialogueSystem.instance.StartCoroutine(Fading(0, speed, immediate));
        rootCG.gameObject.SetActive(false);
        return CO_Hiding;


    }

    private IEnumerator Fading(float alpha, float speed, bool immediate)
    {
        CanvasGroup cg = rootCG;

        if (immediate)
            cg.alpha = alpha;

        while (cg.alpha != alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED * speed);
            yield return null;
        }
        CO_Showing = null;
        CO_Hiding = null;

    }

    public void SetInteractableState(bool active)
    {
        rootCG.interactable = active;
        rootCG.blocksRaycasts = active;
    }
}
