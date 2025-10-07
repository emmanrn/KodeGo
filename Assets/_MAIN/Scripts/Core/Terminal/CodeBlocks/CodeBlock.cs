using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CodeBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image image;
    private TextMeshProUGUI textMeshPro;
    public Transform parentAfterDrag;
    private Canvas canvas;
    private Camera cam;
    public string code;

    void Start()
    {
        image = GetComponent<Image>();
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        code = textMeshPro.text;
        canvas = transform.parent.parent.GetComponentInParent<Canvas>();
        cam = canvas.worldCamera;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        // transform.SetParent(transform.root);
        // transform.SetAsLastSibling();
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        image.raycastTarget = false;
        textMeshPro.raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);

        image.raycastTarget = true;
        textMeshPro.raycastTarget = true;
    }

    public string GetCode() => code;
}
