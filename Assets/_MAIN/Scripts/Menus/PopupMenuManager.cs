using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopupMenuManager : MonoBehaviour
{
    public static PopupMenuManager instance { get; private set; }

    [SerializeField] private PopupMenu[] popups;
    private PopupMenu activePopup = null;
    private PopupMenu activeError = null;
    private PopupMenu activeHint = null;
    private PopupMenu activeVictory = null;

    void Awake()
    {
        instance = this;
    }

    private PopupMenu GetPopupMenu(PopupMenu.PopupType popupType)
    {
        return popups.FirstOrDefault(popup => popup.popupType == popupType);
    }

    public void ShowHintPopup(string message)
    {
        var popup = GetPopupMenu(PopupMenu.PopupType.Hints);
        ShowPopup(popup, message);
        activeHint = popup;
    }
    public void ShowErrorPopup(string message)
    {
        var popup = GetPopupMenu(PopupMenu.PopupType.Error);
        ShowPopup(popup, message);
        activeError = popup;
    }
    public void ShowVictoryPopup(string message)
    {
        var popup = GetPopupMenu(PopupMenu.PopupType.Victory);
        ShowPopup(popup, message);
        activeVictory = popup;
    }

    private void ShowPopup(PopupMenu popup, string message)
    {

        if (popup == null)
            return;

        if (activePopup != null && activePopup != popup)
            activePopup.Hide();

        popup.Show(message);

    }

    public void HideHintPopup()
    {
        if (activeHint != null)
        {
            activeHint.Hide();
            activeHint = null;
        }
    }
    public void HideErrorPopup()
    {
        if (activeError != null)
        {
            activeError.Hide();
            activeError = null;
        }
    }
    public void HideVictoryPopup()
    {
        if (activeVictory != null)
        {
            activeVictory.Hide();
            activeVictory = null;
        }
    }

}
