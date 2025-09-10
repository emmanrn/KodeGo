using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CodeBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image image;
    private TextMeshProUGUI textMeshPro;
    [SerializeField] private Camera camera;

    void Start()
    {
        image = GetComponent<Image>();
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        textMeshPro.raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = (Vector2)camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = transform.parent.position;

        image.raycastTarget = true;
        textMeshPro.raycastTarget = true;
    }
}
