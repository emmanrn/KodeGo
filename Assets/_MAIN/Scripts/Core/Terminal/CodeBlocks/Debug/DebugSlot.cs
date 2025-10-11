using UnityEngine;
using UnityEngine.EventSystems;

public class DebugSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        CodeBlock blockInside = GetComponentInChildren<CodeBlock>();
        GameObject drop = eventData.pointerDrag;
        CodeBlock block = drop.GetComponent<CodeBlock>();


        blockInside.transform.SetParent(block.parentAfterDrag);
        block.parentAfterDrag = transform;


    }
}
