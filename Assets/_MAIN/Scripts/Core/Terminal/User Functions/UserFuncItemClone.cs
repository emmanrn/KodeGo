using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserFuncItemClone : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public Image image;
    public TextMeshProUGUI childTxt;
    public FunctionsArea functionsArea;
    public UserFunc cloneUserFunc;
    public Button button;
    public Transform parent;
    private UserFuncItem parentFunc;
    private Vector3 originalPos;
    public bool selected = false;
    [HideInInspector] public UnityEvent<UserFuncItemClone, bool> EditingEvent;

    public void InitClone(Image origImg, TextMeshProUGUI origChildTxt, UserFunc userFunc, UserFuncItem parentFunc)
    {
        image = GetComponent<Image>();
        childTxt = GetComponentInChildren<TextMeshProUGUI>();
        functionsArea = transform.parent.GetComponentInParent<FunctionsArea>();
        button = GetComponentInChildren<Button>();

        image.sprite = origImg.sprite;
        image.color = origImg.color;
        childTxt.text = origChildTxt.text;
        cloneUserFunc = userFunc;
        parent = transform.parent;
        originalPos = transform.localPosition;
        this.parentFunc = parentFunc;
        functionsArea.funcItemClones.Add(this);
        this.name = functionsArea.funcItemClones.Count.ToString();
        button.gameObject.SetActive(false);


    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        selected = false;
        functionsArea.BeginDrag(this);
        image.raycastTarget = false;
        childTxt.raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

        functionsArea.Swapping();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        selected = false;
        functionsArea.EndDrag();
        transform.localPosition = originalPos;

        image.raycastTarget = true;
        childTxt.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector3 currentPos = transform.localPosition;

        if (!selected)
        {
            currentPos.x += 10;

            transform.localPosition = currentPos;
            button.gameObject.SetActive(true);

            selected = !selected;
            return;

        }
        currentPos.x -= 10;
        transform.localPosition = currentPos;

        button.gameObject.SetActive(false);
        selected = !selected;

    }

    public void DeleteClone()
    {
        Destroy(this.parent.gameObject);
        parentFunc.children.Remove(this);
    }

}
