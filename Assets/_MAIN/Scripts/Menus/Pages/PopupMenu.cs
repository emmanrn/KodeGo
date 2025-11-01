using TMPro;
using UnityEngine;

public class PopupMenu : MonoBehaviour
{
    public enum PopupType { Hints, Error, Victory }
    public PopupType popupType;
    private const string ENTER = "Enter";
    private const string EXIT = "Exit";
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI content;

    public void Show(string message)
    {
        content.text = message;
        anim.Play(ENTER);
    }

    public void Hide()
    {
        anim.Play(EXIT);
    }
}
