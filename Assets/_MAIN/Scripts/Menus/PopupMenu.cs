using TMPro;
using UnityEngine;

public class PopupMenu : MonoBehaviour
{
    public static PopupMenu instance { get; private set; }
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI content;

    void Awake()
    {
        instance = this;
    }

    public void Show(string content)
    {
        this.content.text = content;
        anim.Play("Enter");
    }

    public void Hide()
    {
        anim.Play("Exit");
    }
}
