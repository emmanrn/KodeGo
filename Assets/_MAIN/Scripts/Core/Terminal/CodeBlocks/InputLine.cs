using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputLine : MonoBehaviour, IDropHandler
{
    private TextMeshProUGUI textMeshPro;
    public UnityEvent<string, int> UpdateEvent;
    public string code { get; set; }

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        code = textMeshPro.text;

    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        CodeBlock codeBlock = dropped.GetComponent<CodeBlock>();


        UpdateEvent.Invoke(codeBlock.code, this.transform.GetSiblingIndex());
        textMeshPro.text = codeBlock.code;

    }
}
