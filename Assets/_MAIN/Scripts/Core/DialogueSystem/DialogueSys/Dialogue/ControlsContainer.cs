
using DIALOGUE;
using UnityEngine;

[System.Serializable]
public class ControlsContainer
{
    public CanvasGroup rootCG;
    private CanvasGroupController cgController;

    private bool initialized = false;

    public void Initialize()
    {
        if (initialized)
            return;

        cgController = new CanvasGroupController(DialogueSystem.instance, rootCG);
    }

    public bool isVisible => cgController.isVisible;
    public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
    public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);


}
