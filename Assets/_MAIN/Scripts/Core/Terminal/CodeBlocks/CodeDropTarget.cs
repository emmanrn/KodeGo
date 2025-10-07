using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodeDropTarget : MonoBehaviour, IDropHandler
{
    private TextMeshProUGUI codeText;
    private LayoutElement layout;

    private float padding = 10f;
    public string currentCode { get; set; }

    void Start()
    {
        codeText = GetComponentInChildren<TextMeshProUGUI>();
        layout = GetComponentInChildren<LayoutElement>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        // Check if a CodeBlock was dropped
        CodeBlock block = eventData.pointerDrag.GetComponent<CodeBlock>();
        if (block != null)
        {
            currentCode = block.code;

            // Update the text visually
            codeText.text = currentCode;
            UpdateWidth();
        }
    }

    private void UpdateWidth()
    {
        // Get the preferred width of the TMP text
        float preferred = codeText.preferredWidth + padding;

        // Ensure it doesn't shrink below min width
        layout.preferredWidth = Mathf.Max(layout.minWidth, preferred);
    }

}
