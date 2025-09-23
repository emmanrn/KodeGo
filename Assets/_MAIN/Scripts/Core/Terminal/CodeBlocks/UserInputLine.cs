using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UserInputLine : InputLine
{
    private TMP_InputField inputField;
    public override void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(OnChangeInput);
    }


    public void OnChangeInput(string currentInput)
    {
        UpdateEvent.Invoke(currentInput, this.transform.GetSiblingIndex());
    }


}
