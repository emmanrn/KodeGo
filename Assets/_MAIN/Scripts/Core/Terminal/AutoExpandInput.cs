using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(TMP_InputField))]
public class AutoResizeTMPInput : MonoBehaviour
{
    [SerializeField] private float padding = 10f;
    [SerializeField] private float minWidth = 30f;
    [SerializeField] private float maxWidth = 400f;

    private TMP_InputField inputField;
    private RectTransform rectTransform;
    private TMP_Text textComponent; // <-- use TMP_Text (base class)

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        rectTransform = GetComponent<RectTransform>();
        textComponent = inputField.textComponent; // works now for both TMP and TMPUGUI

        inputField.onValueChanged.AddListener(UpdateWidth);
        UpdateWidth(inputField.text);
    }

    private void UpdateWidth(string currentText)
    {
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate();

        float textWidth = textComponent.textBounds.size.x;
        float newWidth = Mathf.Clamp(textWidth + padding, minWidth, maxWidth);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        // Force TMP to reposition the caret and update the text mesh
        if (inputField.isFocused)
        {
            inputField.ForceLabelUpdate();
            inputField.caretPosition = inputField.text.Length;
        }
    }
}
