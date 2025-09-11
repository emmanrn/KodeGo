using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LineSlot : MonoBehaviour
{
    private TextMeshProUGUI[] texts;
    private InputLine[] inputLines;

    public string[] lines { get; set; }


    void Start()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
        inputLines = GetComponentsInChildren<InputLine>();
        lines = new string[texts.Length];

        for (int i = 0; i < texts.Length; i++)
        {
            lines[i] = texts[i].text;
        }

        if (inputLines.Length > 0)
            for (int i = 0; i < inputLines.Length; i++)
                inputLines[i].UpdateEvent.AddListener(OnUpdate);

    }

    private void OnUpdate(string newInput, int index) => lines[index] = newInput;

    public int GetNumberOfTabs()
    {
        int num = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "\t")
                num++;
        }

        return num;
    }
}
