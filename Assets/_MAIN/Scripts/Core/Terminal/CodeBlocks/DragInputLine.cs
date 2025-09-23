using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragInputLine : InputLine, IDropHandler
{
    private TextMeshProUGUI textMeshPro;

    public override void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        CodeBlock codeBlock = dropped.GetComponent<CodeBlock>();


        UpdateEvent.Invoke(codeBlock.code, this.transform.GetSiblingIndex());
        textMeshPro.text = codeBlock.code;
    }
}
