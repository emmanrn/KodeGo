using System.Collections;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    public IInteractable interactableRange = null; // Closest interactable
    public GameObject interactionIcon;
    [SerializeField] private Animator anim;
    private Coroutine closeRoutine;

    void Start()
    {
        interactionIcon.SetActive(false);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.isInteractable())
        {
            interactableRange = interactable;

            if (closeRoutine != null)
            {
                StopCoroutine(closeRoutine);
                closeRoutine = null;
            }

            if (!interactionIcon.activeSelf)
                interactionIcon.SetActive(true);

            if (anim.isActiveAndEnabled)
                anim.Play("Open");
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableRange)
        {
            interactableRange = null;
            if (closeRoutine != null)
            {
                StopCoroutine(closeRoutine);
                closeRoutine = null;
            }

            if (gameObject.activeInHierarchy)
                closeRoutine = StartCoroutine(CloseIcon());
        }
    }

    private IEnumerator CloseIcon()
    {
        anim.Play("Close");

        float closeAnimLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(closeAnimLength);

        interactionIcon.SetActive(false);
    }

}
