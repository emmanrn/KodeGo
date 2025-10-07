using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragInputLineSlot : InputLine, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        CodeBlock blockInside = GetComponentInChildren<CodeBlock>();
        GameObject drop = eventData.pointerDrag;
        CodeBlock block = drop.GetComponent<CodeBlock>();

        Transform originSlot = block.parentAfterDrag;
        DragInputLineSlot originSlotLine = originSlot.GetComponent<DragInputLineSlot>();

        blockInside.transform.SetParent(block.parentAfterDrag);
        UpdateEvent.Invoke(block.code, this.transform.GetSiblingIndex());
        block.parentAfterDrag = transform;

        originSlotLine.UpdateEvent.Invoke(blockInside.code, originSlotLine.transform.GetSiblingIndex());
        UpdateEvent.Invoke(block.code, this.transform.GetSiblingIndex());

    }
}
