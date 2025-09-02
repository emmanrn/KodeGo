using UnityEngine;
using UnityEngine.EventSystems;

public class UserFuncSlot : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            UserFuncItem funcItem = eventData.pointerDrag.GetComponent<UserFuncItem>();
            funcItem.parentAfterDrag = transform;
        }
    }

}
