using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class FunctionsArea : MonoBehaviour
{
    public List<UserFuncItemClone> funcItemClones;
    public UserFuncItemClone selectedFunc;
    public GameObject funcCloneSlotPrefab;
    private bool isCrossing = false;

    public void BeginDrag(UserFuncItemClone clone)
    {
        selectedFunc = clone;
    }

    public void EndDrag()
    {
        selectedFunc = null;
    }

    public void Swapping()
    {
        for (int i = 0; i < funcItemClones.Count; i++)
        {
            if (funcItemClones[i] == selectedFunc) continue;

            if (selectedFunc.transform.position.y < funcItemClones[i].transform.position.y)
            {
                if (selectedFunc.transform.parent.GetSiblingIndex() < funcItemClones[i].transform.parent.GetSiblingIndex())
                {
                    Swap(i);
                    break;
                }
            }
            if (selectedFunc.transform.position.y > funcItemClones[i].transform.position.y)
            {
                if (selectedFunc.transform.parent.GetSiblingIndex() > funcItemClones[i].transform.parent.GetSiblingIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }


    }

    void Update()
    {
        if (selectedFunc == null)
            return;

        if (isCrossing)
            return;


    }

    private void Swap(int index)
    {
        isCrossing = true;

        Transform focusedFunc = selectedFunc.transform.parent;
        Transform crossedFunc = funcItemClones[index].transform.parent;


        funcItemClones[index].transform.SetParent(focusedFunc);
        funcItemClones[index].transform.localPosition = Vector3.zero;
        selectedFunc.transform.SetParent(crossedFunc);


        isCrossing = false;

    }
}
