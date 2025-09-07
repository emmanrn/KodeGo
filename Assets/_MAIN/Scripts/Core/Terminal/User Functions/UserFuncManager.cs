using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UserFuncManager : MonoBehaviour
{
    public TMP_InputField terminalInput;
    public GameObject userFuncPrefab;
    public UserFuncSlot[] functionItemSlots;
    [Header("User Funcs Lists")]
    public List<UserFunc> userFuncs = new();
    public TextMeshProUGUI text;
    [SerializeField] private Interpreter interpreter;
    public UserFuncItemClone[] funcItem;
    public FunctionsArea functionsArea;
    public GameObject saveBtn;
    private UserFuncItem editingItem;


    void Start()
    {
        saveBtn.SetActive(false);
    }

    public void AddFunc()
    {
        if (terminalInput.text == "" || terminalInput.text == null)
        {
            Debug.LogError("Can't add empty code");
            return;

        }

        for (int i = 0; i < functionItemSlots.Length; i++)
        {
            UserFuncSlot slot = functionItemSlots[i];
            UserFuncItem funcInSlot = slot.GetComponentInChildren<UserFuncItem>();

            if (funcInSlot == null)
            {
                UserFunc newFunc = new UserFunc(terminalInput.text, userFuncs.Count);
                userFuncs.Add(newFunc);

                GameObject userFuncItemObj = Instantiate(userFuncPrefab, slot.transform);
                userFuncItemObj.transform.localPosition = Vector3.zero;
                UserFuncItem userFuncItem = userFuncItemObj.GetComponent<UserFuncItem>();

                userFuncItem.InitItem(newFunc, functionsArea);



                userFuncItem.OnClickEdit.AddListener(Editing);

                return;
            }
        }


    }

    public void ExecFuncs()
    {
        FunctionsArea funcArea = GetComponentInChildren<FunctionsArea>();
        funcItem = funcArea.GetComponentsInChildren<UserFuncItemClone>();

        if (funcItem == null || funcItem.Length == 0)
        {
            Debug.LogWarning("No functions to execute");
            return;
        }

        interpreter.ClearOutput();

        foreach (var func in funcItem)
        {
            if (func == null || func.cloneUserFunc == null) continue;

            string codeBlock = func.cloneUserFunc.code;

            if (!string.IsNullOrWhiteSpace(codeBlock))
            {
                try
                {
                    interpreter.ExecuteCode(codeBlock);

                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error executing block:\n{codeBlock}\n{ex.Message}");
                }
            }
        }

        string result = interpreter.GetOutput();
        if (!string.IsNullOrEmpty(result))
            DisplayResult(result);


    }

    public void Editing(UserFuncItem funcItem, bool editing)
    {
        editingItem = funcItem;
        UserFunc userFunc = funcItem.userFunc;
        terminalInput.ActivateInputField();
        terminalInput.text = userFunc.code;

        saveBtn.SetActive(true);
    }

    public void SaveChangesFunc()
    {

        editingItem.userFunc.code = terminalInput.text;
        editingItem.childTxt.text = terminalInput.text;
        Debug.Log(editingItem.userFunc.code);




        editingItem.UpdateClones(editingItem.userFunc);
        terminalInput.text = "";

        saveBtn.SetActive(false);
    }

    public void DisplayResult(string res)
    {
        text.text = res;
    }
}
