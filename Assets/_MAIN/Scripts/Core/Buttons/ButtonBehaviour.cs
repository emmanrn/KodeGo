using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static ButtonBehaviour selectedButton = null;
    public Animator anim;

    public void OnPointerExit(PointerEventData eventData)
    {
        if (anim != null)
            anim.Play("Exit");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (selectedButton != null && selectedButton != this)
        {
            selectedButton.OnPointerExit(null);
        }

        if (anim != null)
            anim.Play("Enter");

        AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResource(FilePaths.resources_sfx, "button_hover"));
        selectedButton = this;
    }
}
