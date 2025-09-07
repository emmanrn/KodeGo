using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserFuncItem : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    public Transform parentAfterDrag;
    public Image image;
    public TextMeshProUGUI childTxt;
    public UserFunc userFunc;
    public GameObject funcClonePrefab;
    public GameObject cloneItemPrefab;
    public FunctionsArea functionsArea;
    public Button saveBtn;
    public Button delBtn;
    GameObject newfuncObj;
    GameObject cloneObj;
    public UserFuncItemClone cloneItem;
    // TODO
    // try to use inheritance for this one.
    // also make it getter and setter.
    public List<UserFuncItemClone> children;
    public bool selected = false;
    public bool editing = false;
    [HideInInspector] public UnityEvent<UserFuncItem, bool> OnClickEdit;


    public void InitItem(UserFunc userFunc, FunctionsArea funcArea)
    {
        image = GetComponent<Image>();
        childTxt = GetComponentInChildren<TextMeshProUGUI>();
        saveBtn.gameObject.SetActive(false);
        delBtn.gameObject.SetActive(false);

        parentAfterDrag = transform.parent;

        functionsArea = funcArea;
        this.userFunc = userFunc;
        string funcName = GetPyFuncName(userFunc.code);
        this.userFunc.name = funcName;
        childTxt.text = this.userFunc.name;

    }
    private string GetPyFuncName(string code)
    {
        string[] lines = code.Split(new[] { '\n', '\r', });
        string funcName = "";

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmed = lines[i].TrimStart();

            if (trimmed.StartsWith("def "))
            {
                int start = 4;
                int end = trimmed.IndexOf('(', start);
                if (end > start)
                {
                    funcName = trimmed.Substring(start, end - start).Trim();
                    Debug.Log(funcName);

                }
            }
            else
            {
                // TODO
                //  fix this to where if the block is just calling a function then just 
                //  get that code as name instead
                funcName = code;
                return funcName;
            }





        }
        return funcName;


    }

    void Update()
    {
        saveBtn.gameObject.SetActive(selected || editing ? true : false);
        delBtn.gameObject.SetActive(selected || editing ? true : false);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        newfuncObj = Instantiate(funcClonePrefab, Input.mousePosition, Quaternion.identity, functionsArea.gameObject.transform);
        cloneObj = Instantiate(cloneItemPrefab, newfuncObj.transform);
        cloneItem = cloneObj.GetComponent<UserFuncItemClone>();

        cloneItem.InitClone(image, childTxt, userFunc, this);
        children.Add(cloneItem);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        selected = !selected;
    }


    public void OnEditing()
    {
        editing = true;
        OnClickEdit.Invoke(this, editing);
        Debug.Log("editing");

    }

    public void CancelEdit()
    {
        editing = false;
        selected = false;

    }

    public void OnDelete()
    {

        Destroy(this.gameObject);

        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                Destroy(children[i].transform.parent.gameObject);


            }
            children.Clear();

        }

    }

    public void UpdateClones(UserFunc userFunc)
    {
        editing = false;
        selected = false;
        string newFuncName = GetPyFuncName(userFunc.code);
        this.userFunc.name = newFuncName;
        childTxt.text = newFuncName;

        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].cloneUserFunc.code = userFunc.code;
                children[i].childTxt.text = newFuncName;
                children[i].cloneUserFunc.name = newFuncName;
            }
            //TODO change name of func block when updated



        }
    }





}
